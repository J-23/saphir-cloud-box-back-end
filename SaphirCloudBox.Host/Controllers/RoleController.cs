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
    [Route("api/role")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(Policy = "Bearer")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogService _logService;

        public RoleController(IRoleService roleService, ILogService logService)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult> GetRoleList()
        {
            var roles = await _roleService.GetAll();
            return Ok(roles);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> AddDepartment([FromBody]AddRoleDto roleDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _roleService.Add(roleDto);
            await _logService.Add(Enums.LogType.Create, $"Role with name = {roleDto.Name} was created successfully by user = {userId}");
            return Ok();
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateDepartment([FromBody]RoleDto roleDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _roleService.Update(roleDto);
            await _logService.Add(Enums.LogType.Update, $"Role with id = {roleDto.Id} was updated successfully by user = {userId}");
            return Ok();
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveDepartment([FromBody]RemoveRoleDto roleDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _roleService.Remove(roleDto);
            await _logService.Add(Enums.LogType.Remove, $"Role with id = {roleDto.Id} was removed successfully by user = {userId}");
            return Ok();
        }
    }
}