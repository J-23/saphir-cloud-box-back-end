using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaphirCloudBox.Host.Infractructure;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Dtos.Permission;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Services;

namespace SaphirCloudBox.Host.Controllers
{
    [Route("api/file-storage")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(Policy = "Bearer")]
    public class FileStorageController : BaseController
    {
        private readonly IFileStorageService _fileStorageService;

        public FileStorageController(IFileStorageService fileStorageService, ILogService logService): base(logService)
        {
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        }

        [HttpGet]
        [Route("{parentId}")]
        public async Task<ActionResult> GetFolders(int parentId = 0)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            try
            {
                var fileStorages = await _fileStorageService.GetByParentId(parentId, UserId, ClientId);
                return Ok(fileStorages);
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundMessage(LogMessage.FolderEntityName, parentId));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND.ToString());
            }
            catch(Exception ex)
            {
                await AddLog(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("add/folder")]
        public async Task<ActionResult> AddFolder([FromBody]AddFolderDto folderDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.AddFolder(folderDto, UserId, ClientId);

                await AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByNameMessage(LogMessage.FolderEntityName, 
                    folderDto.Name, LogMessage.CreateAction, UserId));

                return Ok();
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundParentMessage(LogMessage.FolderEntityName, folderDto.ParentId, 
                    LogMessage.FolderEntityName, folderDto.Name));

                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND.ToString());
            }
            catch (FoundSameObjectException)
            {
                await AddLog(Enums.LogType.SameObject, LogMessage.CreateFoundSameObjectMessage(LogMessage.FolderEntityName, folderDto.Name));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (Exception ex)
            {
                await AddLog(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("update/folder")]
        public async Task<ActionResult> UpdateFolder([FromBody]UpdateFolderDto folderDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.UpdateFolder(folderDto, UserId, ClientId);
                await AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.FolderEntityName, folderDto.Id, LogMessage.CreateAction, UserId));
                return Ok();
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundMessage(LogMessage.FolderEntityName, folderDto.Id));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND.ToString());
            }
            catch (FoundSameObjectException)
            {
                await AddLog(Enums.LogType.SameObject, LogMessage.CreateFoundSameObjectMessage(LogMessage.FolderEntityName, folderDto.Name));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (UpdateException)
            {
                await AddLog(Enums.LogType.NoAccess, LogMessage.CreateNoAccessMessage(LogMessage.FolderEntityName, folderDto.Id, UserId));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
            catch (Exception ex)
            {
                await AddLog(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("remove/folder")]
        public async Task<ActionResult> RemoveFolder([FromBody]RemoveFileStorageDto folderDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.RemoveFolder(folderDto, UserId, ClientId);
                await AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.FolderEntityName, folderDto.Id, LogMessage.RemoveAction, UserId));
                return Ok();
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundMessage(LogMessage.FolderEntityName, folderDto.Id));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND.ToString());
            }
            catch(RemoveException)
            {
                await AddLog(Enums.LogType.NoAccess, LogMessage.CreateNoAccessMessage(LogMessage.FolderEntityName, folderDto.Id, UserId));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
            catch (Exception ex)
            {
                await AddLog(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("add/file")]
        public async Task<ActionResult> AddFile([FromBody]AddFileDto fileDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.AddFile(fileDto, UserId, ClientId);
                await AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByNameMessage(LogMessage.FileEntityName, fileDto.Name, LogMessage.CreateAction, UserId));
                return Ok();
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundParentMessage(LogMessage.FolderEntityName, fileDto.ParentId, LogMessage.FileEntityName, fileDto.Name));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND.ToString());
            }
            catch (FoundSameObjectException)
            {
                await AddLog(Enums.LogType.SameObject, LogMessage.CreateFoundSameObjectMessage(LogMessage.FileEntityName, fileDto.Name));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (Exception ex)
            {
                await AddLog(Enums.LogType.NotFound, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpGet]
        [Route("download/file/{id}/{ownerId}/{clientId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFile(int id, int ownerId, int clientId)
        {
            try
            {
                var fileDto = await _fileStorageService.GetFileById(id, ownerId, clientId);
                var contentType = Constants.Extension.TYPES[Path.GetExtension(fileDto.Extension).ToLowerInvariant()];

                return File(fileDto.Buffer, contentType, fileDto.Name + fileDto.Extension);
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundMessage(LogMessage.FileEntityName, id));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND.ToString());
            }
            catch (Exception ex)
            {
                await AddLog(Enums.LogType.NotFound, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("update/file")]
        public async Task<IActionResult> UpdateFile([FromBody]UpdateFileDto fileDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.UpdateFile(fileDto, UserId, ClientId);
                await AddLog(Enums.LogType.Update, LogMessage.CreateSuccessByIdMessage(LogMessage.FileEntityName, fileDto.Id, LogMessage.UpdateAction, UserId));
                return Ok();
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundMessage(LogMessage.FileEntityName, fileDto.Id));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND.ToString());
            }
            catch (UpdateException)
            {
                await AddLog(Enums.LogType.NoAccess, LogMessage.CreateNoAccessMessage(LogMessage.FileEntityName, fileDto.Id, UserId));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
            catch (FoundSameObjectException)
            {
                await AddLog(Enums.LogType.SameObject, LogMessage.CreateFoundSameObjectMessage(LogMessage.FileEntityName, fileDto.Name));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (Exception ex)
            {
                await AddLog(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("remove/file")]
        public async Task<ActionResult> RemoveFile([FromBody]RemoveFileStorageDto fileDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.RemoveFile(fileDto, UserId, ClientId);
                await AddLog(Enums.LogType.Remove, LogMessage.CreateSuccessByIdMessage(LogMessage.FileEntityName, fileDto.Id, LogMessage.RemoveAction, UserId));
                return Ok();
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundMessage(LogMessage.FileEntityName, fileDto.Id));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND.ToString());
            }
            catch (RemoveException)
            {
                await AddLog(Enums.LogType.NoAccess, LogMessage.CreateNoAccessMessage(LogMessage.FileEntityName, fileDto.Id, UserId));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
            catch (Exception ex)
            {
                await AddLog(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("add/permission")]
        public async Task<ActionResult> AddPermission([FromBody]AddPermissionDto permissionDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.AddPermission(permissionDto, UserId, ClientId);
                await AddLog(Enums.LogType.Create, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.CreateVerb, UserId));
                return Ok();
            }
            catch (AddException)
            {
                await AddLog(Enums.LogType.NoAccess, LogMessage.CreateUnavailablePermissionMessage(UserId, LogMessage.CreateVerb, permissionDto.RecipientEmail));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.ADD_ERROR.ToString());
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundByEmailMessage(LogMessage.UserEntityName, permissionDto.RecipientEmail));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND_USER.ToString());
            }
            catch (Exception ex)
            {
                await AddLog(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("update/permission")]
        public async Task<ActionResult> UpdatePermission([FromBody]UpdatePermissionDto permissionDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.UpdatePermission(permissionDto, UserId, ClientId);
                await AddLog(Enums.LogType.Update, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.UpdateVerb, UserId));
                return Ok();
            }
            catch (UpdateException)
            {
                await AddLog(Enums.LogType.NoAccess, LogMessage.CreateUnavailablePermissionMessage(UserId, LogMessage.CreateVerb, permissionDto.RecipientEmail));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.UPDATE_ERROR.ToString());
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundByEmailMessage(LogMessage.UserEntityName, permissionDto.RecipientEmail));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND_USER.ToString());
            }
            catch (Exception ex)
            {
                await AddLog(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("remove/permission")]
        public async Task<ActionResult> RemovePermission([FromBody]RemovePermissionDto permissionDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.RemovePermission(permissionDto, UserId, ClientId);
                await AddLog(Enums.LogType.Remove, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.UpdateVerb, UserId));
                return Ok();
            }
            catch (RemoveException)
            {
                await AddLog(Enums.LogType.NoAccess, LogMessage.CreateUnavailablePermissionMessage(UserId, LogMessage.CreateVerb, permissionDto.RecipientEmail));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.REMOVE_ERROR.ToString());
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundByEmailMessage(LogMessage.UserEntityName, permissionDto.RecipientEmail));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND_USER.ToString());
            }
            catch (Exception ex)
            {
                await AddLog(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }
    }
}