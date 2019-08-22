using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaphirCloudBox.Host.Infractructure;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Dtos.Permission;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Services;

namespace SaphirCloudBox.Host.Controllers
{
    [Route("api/shared-file")]
    [ApiController]
    public class SharedFileController : BaseController
    {
        private readonly ISharedFileService _sharedFileService;

        public SharedFileController(ILogService logService,
            ISharedFileService sharedFileService) 
            : base(logService)
        {
            _sharedFileService = sharedFileService ?? throw new ArgumentNullException(nameof(sharedFileService));
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult> GetSharedFiles()
        {
            var sharedFiles = await _sharedFileService.GetByUserId(UserId);
            return Ok(sharedFiles);
        }

        [HttpPost]
        [Route("add-permission")]
        public async Task<ActionResult> AddPermission([FromBody]AddPermissionDto permissionDto)
        {
            try
            {
                await _sharedFileService.AddPermission(permissionDto, UserId);
                await AddLog(Enums.LogType.Create, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.CreateAction, UserId));
                return Ok();
            }
            catch (AddException)
            {
                await AddLog(Enums.LogType.Create, LogMessage.CreateUnavailablePermissionMessage(UserId, LogMessage.CreateVerb, permissionDto.RecipientEmail));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.ADD_ERROR.ToString());
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }
    }
}