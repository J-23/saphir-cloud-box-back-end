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

            try
            {
                await _userService.Add(userDto, _appSettings.CommonPassword);
                await _logService.Add(Enums.LogType.Create, $"User with email = {userDto.Email} was created successfully by user = {userId}");
                return Ok();
            }
            catch (FoundSameObjectException)
            {
                await _logService.Add(Enums.LogType.SameObject, $"Failed to create user with email = {userDto.Email}. User already exists with that email");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (NotFoundDependencyObjectException)
            {
                await _logService.Add(Enums.LogType.Error, $"Role with Id = {userDto.RoleId} for user with email = {userDto.Email} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT_DEPENDENCY_OBJECT.ToString());
            }
            catch (AddException)
            {
                await _logService.Add(Enums.LogType.Error, $"Error creating user with email = {userDto.Email} by user manager");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.ADD_ERROR.ToString());
            }
            catch (Exception ex)
            {
                await _logService.Add(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
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

            try
            {
                await _userService.Update(userDto, _appSettings.CommonPassword);
                await _logService.Add(Enums.LogType.Update, $"User with id = {userDto.Id} was updated successfully by user = {userId}");
                return Ok();
            }
            catch (NotFoundException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"User with Id = {userDto.Id} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (NotFoundDependencyObjectException)
            {
                await _logService.Add(Enums.LogType.Error, $"Role with Id = {userDto.RoleId} for user with id = {userDto.Id} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT_DEPENDENCY_OBJECT.ToString());
            }
            catch (FoundSameObjectException)
            {
                await _logService.Add(Enums.LogType.SameObject, $"Failed to update user with id = {userDto.Id} and email = {userDto.Email}. User already exists with that email");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (UpdateException)
            {
                await _logService.Add(Enums.LogType.Error, $"Error updating user with id = {userDto.Id} by user manager");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.UPDATE_ERROR.ToString());
            }
            catch (Exception ex)
            {
                await _logService.Add(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
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

            try
            {
                await _userService.Remove(userDto);
                await _logService.Add(Enums.LogType.Remove, $"User with id = {userDto.Id} was removed successfully by user = {userId}");
                return Ok();
            }
            catch (NotFoundException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"User with Id = {userDto.Id} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (RemoveException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"Error removing user with id = {userDto.Id} by user manager");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.REMOVE_ERROR.ToString());
            }
            catch (Exception ex)
            {
                await _logService.Add(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }
    }
}