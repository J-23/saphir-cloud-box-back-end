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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;
        private readonly ILogService _logService;

        public UserController(IUserService userService, AppSettings appSettings, ILogService logService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult> GetUserList()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> AddUser([FromBody]AddUserDto userDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _userService.Add(userDto, _appSettings.CommonPassword);
            await _logService.Add(Enums.LogType.Create, $"User with email = {userDto.Email} was created successfully by user = {userId}");
            return Ok();
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateDepartment([FromBody]UpdateUserDto userDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _userService.Update(userDto, _appSettings.CommonPassword);
            await _logService.Add(Enums.LogType.Update, $"User with id = {userDto.Id} was updated successfully by user = {userId}");
            return Ok();
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveDepartment([FromBody]RemoveUserDto userDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _userService.Remove(userDto);
            await _logService.Add(Enums.LogType.Remove, $"User with id = {userDto.Id} was removed successfully by user = {userId}");
            return Ok();
        }
    }
}