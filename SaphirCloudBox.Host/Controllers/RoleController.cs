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

            try
            {
                await _roleService.Add(roleDto);
                await _logService.Add(Enums.LogType.Create, $"Role with name = {roleDto.Name} was created successfully by user = {userId}");
                return Ok();
            }
            catch (FoundSameObjectException)
            {
                await _logService.Add(Enums.LogType.SameObject, $"Failed to create role with name = {roleDto.Name}. Role already exists with that name");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (AddException)
            {
                await _logService.Add(Enums.LogType.Error, $"Error creating role with name = {roleDto.Name} by role manager");
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
        public async Task<ActionResult> UdpateDepartment([FromBody]RoleDto roleDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            try
            {
                await _roleService.Update(roleDto);
                await _logService.Add(Enums.LogType.Update, $"Role with id = {roleDto.Id} was updated successfully by user = {userId}");
                return Ok();
            }
            catch (NotFoundException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"Role with Id = {roleDto.Id} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (FoundSameObjectException)
            {
                await _logService.Add(Enums.LogType.SameObject, $"Failed to update role with id = {roleDto.Id} and name = {roleDto.Name}. Role already exists with that name");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (UpdateException)
            {
                await _logService.Add(Enums.LogType.Error, $"Error updating role with id = {roleDto.Id} by role manager");
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
        public async Task<ActionResult> RemoveDepartment([FromBody]RemoveRoleDto roleDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            try
            {
                await _roleService.Remove(roleDto);
                await _logService.Add(Enums.LogType.Remove, $"Role with id = {roleDto.Id} was removed successfully by user = {userId}");
                return Ok();
            }
            catch (NotFoundException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"Role with Id = {roleDto.Id} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (ExistDependencyException)
            {
                await _logService.Add(Enums.LogType.Error, $"Role with Id = {roleDto.Id} has users");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.EXIST_DEPENDENCY_ERROR.ToString());
            }
            catch (RemoveException)
            {
                await _logService.Add(Enums.LogType.Error, $"Error removing role with id = {roleDto.Id} by role manager");
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