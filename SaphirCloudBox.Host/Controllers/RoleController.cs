using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
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
            try
            {
                await _roleService.Add(roleDto);
                return Ok();
            }
            catch (FoundSameObjectException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (AddException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.ADD_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateDepartment([FromBody]RoleDto roleDto)
        {
            try
            {
                await _roleService.Update(roleDto);
                return Ok();
            }
            catch (NotFoundException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
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
        public async Task<ActionResult> RemoveDepartment([FromBody]RemoveRoleDto roleDto)
        {
            try
            {
                await _roleService.Remove(roleDto);
                return Ok();
            }
            catch (NotFoundException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (ExistDependencyException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.EXIST_DEPENDENCY_ERROR.ToString());
            }
            catch (RemoveException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.REMOVE_ERROR.ToString());
            }
        }
    }
}