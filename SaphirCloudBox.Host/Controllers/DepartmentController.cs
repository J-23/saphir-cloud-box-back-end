using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaphirCloudBox.Host.Infractructure;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Services;

namespace SaphirCloudBox.Host.Controllers
{
    [Route("api/department")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(Policy = "Bearer")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogService _logService;

        public DepartmentController(IDepartmentService departmentService, ILogService logService)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult> GetDepartmentList()
        {
            var departments = await _departmentService.GetAll();
            return Ok(departments);
        }

        [HttpGet]
        [Route("list/client/{clientId}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetDepartmentListByClientId(int clientId)
        {
            var departments = await _departmentService.GetByClientId(clientId);
            return Ok(departments);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> AddDepartment([FromBody]AddDepartmentDto departmentDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _departmentService.Add(departmentDto);
            await _logService.Add(Enums.LogType.Create, $"Department with name = {departmentDto.Name} was created successfully by user = {userId}");
            return Ok();
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateDepartment([FromBody]UpdateDepartmentDto departmentDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _departmentService.Update(departmentDto);
            await _logService.Add(Enums.LogType.Update, $"Department with id = {departmentDto.Id} was updated successfully by user = {userId}");
            return Ok();
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveDepartment([FromBody]RemoveDepartmentDto departmentDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _departmentService.Remove(departmentDto);
            await _logService.Add(Enums.LogType.Remove, $"Department with id = {departmentDto.Id} was removed successfully by user = {userId}");
            return Ok();
        }
    }
}