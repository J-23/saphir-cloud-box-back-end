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
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService, ILogService logService): base(logService)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult> GetDepartmentList()
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var departments = await _departmentService.GetAll(UserId, ClientId);
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
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _departmentService.Add(departmentDto);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByNameMessage(LogMessage.DepartmentEntityName, departmentDto.Name, LogMessage.CreateAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateDepartment([FromBody]UpdateDepartmentDto departmentDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _departmentService.Update(departmentDto);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.DepartmentEntityName, departmentDto.Id, LogMessage.UpdateAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveDepartment([FromBody]RemoveDepartmentDto departmentDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _departmentService.Remove(departmentDto);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.DepartmentEntityName, departmentDto.Id, LogMessage.RemoveAction, UserId));
            return Ok();
        }
    }
}