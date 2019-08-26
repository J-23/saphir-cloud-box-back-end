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
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService, ILogService logService): base(logService)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
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
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _roleService.Add(roleDto);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByNameMessage( LogMessage.RoleEntityName, roleDto.Name, LogMessage.CreateAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateDepartment([FromBody]RoleDto roleDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _roleService.Update(roleDto);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.RoleEntityName, roleDto.Id, LogMessage.UpdateAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveDepartment([FromBody]RemoveRoleDto roleDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _roleService.Remove(roleDto);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.RoleEntityName, roleDto.Id, LogMessage.RemoveAction, UserId));
            return Ok();
        }
    }
}