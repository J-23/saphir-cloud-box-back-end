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
    [Route("api/department")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
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
        public async Task<ActionResult> GetDepartmentListByClientId(int clientId)
        {
            var departments = await _departmentService.GetByClientId(clientId);
            return Ok(departments);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> AddDepartment([FromBody]AddDepartmentDto departmentDto)
        {
            try
            {
                await _departmentService.Add(departmentDto);
                return Ok();
            }
            catch (NotFoundException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT_DEPENDENCY_OBJECT.ToString());
            }
            catch (FoundSameObjectException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateDepartment([FromBody]UpdateDepartmentDto departmentDto)
        {
            try
            {
                await _departmentService.Update(departmentDto);
                return Ok();
            }
            catch (NotFoundException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (UpdateException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT_DEPENDENCY_OBJECT.ToString());
            }
            catch (FoundSameObjectException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveDepartment([FromBody]RemoveDepartmentDto departmentDto)
        {
            try
            {
                await _departmentService.Remove(departmentDto);
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