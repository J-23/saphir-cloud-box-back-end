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

        public UserController(IUserService userService, AppSettings appSettings)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
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
            try
            {
                await _userService.Add(userDto, _appSettings.CommonPassword);
                return Ok();
            }
            catch (FoundSameObjectException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (NotFoundDependencyObjectException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT_DEPENDENCY_OBJECT.ToString());
            }
            catch (AddException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.ADD_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateDepartment([FromBody]UpdateUserDto userDto)
        {
            try
            {
                await _userService.Update(userDto, _appSettings.CommonPassword);
                return Ok();
            }
            catch (NotFoundException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (NotFoundDependencyObjectException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT_DEPENDENCY_OBJECT.ToString());
            }
            catch (FoundSameObjectException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (UpdateException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.UPDATE_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveDepartment([FromBody]RemoveUserDto userDto)
        {
            try
            {
                await _userService.Remove(userDto);
                return Ok();
            }
            catch (NotFoundException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (RemoveException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.REMOVE_ERROR.ToString());
            }
        }
    }
}