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
    [Route("api/user")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(Policy = "Bearer")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;

        public UserController(IUserService userService, AppSettings appSettings, ILogService logService): base(logService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult> GetUserList()
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var users = await _userService.GetAll(UserId, ClientId);
            return Ok(users);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> AddUser([FromBody]AddUserDto userDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _userService.Add(userDto, _appSettings.CommonPassword);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByNameMessage(LogMessage.UserEntityName, userDto.Email, LogMessage.CreateAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateDepartment([FromBody]UpdateUserDto userDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _userService.Update(userDto, _appSettings.CommonPassword);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.UserEntityName, userDto.Id, LogMessage.UpdateAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveDepartment([FromBody]RemoveUserDto userDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _userService.Remove(userDto);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.UserEntityName, userDto.Id, LogMessage.RemoveAction, UserId));
            return Ok();
        }
    }
}