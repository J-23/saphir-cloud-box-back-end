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

        public FileStorageController(IFileStorageService fileStorageService, ILogService logService) : base(logService)
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

            var fileStorages = await _fileStorageService.GetByParentId(parentId, UserId, ClientId);
            return Ok(fileStorages);
        }

        [HttpPost]
        [Route("add/folder")]
        public async Task<ActionResult> AddFolder([FromBody]AddFolderDto folderDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _fileStorageService.AddFolder(folderDto, UserId, ClientId);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByNameMessage(LogMessage.FolderEntityName, folderDto.Name, LogMessage.CreateAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("update/folder")]
        public async Task<ActionResult> UpdateFolder([FromBody]UpdateFolderDto folderDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _fileStorageService.UpdateFolder(folderDto, UserId, ClientId);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.FolderEntityName, folderDto.Id, LogMessage.CreateAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("remove/folder")]
        public async Task<ActionResult> RemoveFolder([FromBody]RemoveFileStorageDto folderDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _fileStorageService.RemoveFolder(folderDto, UserId, ClientId);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.FolderEntityName, folderDto.Id, LogMessage.RemoveAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("add/file")]
        public async Task<ActionResult> AddFile([FromBody]AddFileDto fileDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }


            await _fileStorageService.AddFile(fileDto, UserId, ClientId);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByNameMessage(LogMessage.FileEntityName, fileDto.Name, LogMessage.CreateAction, UserId));
            return Ok();
        }

        [HttpGet]
        [Route("download/file/{id}/{ownerId}/{clientId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFile(int id, int ownerId, int clientId)
        {
            var fileDto = await _fileStorageService.GetFileById(id, ownerId, clientId);
            var contentType = Constants.Extension.TYPES[Path.GetExtension(fileDto.Extension).ToLowerInvariant()];

            return File(fileDto.Buffer, contentType, fileDto.Name + fileDto.Extension);
        }

        [HttpPost]
        [Route("update/file")]
        public async Task<IActionResult> UpdateFile([FromBody]UpdateFileDto fileDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }



            await _fileStorageService.UpdateFile(fileDto, UserId, ClientId);
            AddLog(Enums.LogType.Update, LogMessage.CreateSuccessByIdMessage(LogMessage.FileEntityName, fileDto.Id, LogMessage.UpdateAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("remove/file")]
        public async Task<ActionResult> RemoveFile([FromBody]RemoveFileStorageDto fileDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }


            await _fileStorageService.RemoveFile(fileDto, UserId, ClientId);
            AddLog(Enums.LogType.Remove, LogMessage.CreateSuccessByIdMessage(LogMessage.FileEntityName, fileDto.Id, LogMessage.RemoveAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("add/permission")]
        public async Task<ActionResult> AddPermission([FromBody]AddPermissionDto permissionDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _fileStorageService.AddPermission(permissionDto, UserId, ClientId);
            AddLog(Enums.LogType.Create, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.CreateVerb, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("update/permission")]
        public async Task<ActionResult> UpdatePermission([FromBody]UpdatePermissionDto permissionDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _fileStorageService.UpdatePermission(permissionDto, UserId, ClientId);
            AddLog(Enums.LogType.Update, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.UpdateVerb, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("remove/permission")]
        public async Task<ActionResult> RemovePermission([FromBody]RemovePermissionDto permissionDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _fileStorageService.RemovePermission(permissionDto, UserId, ClientId);
            AddLog(Enums.LogType.Remove, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.UpdateVerb, UserId));
            return Ok();
        }

        [HttpGet]
        [Route("shared-with-me")]
        public async Task<ActionResult> GetSharedFiles()
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var storages = await _fileStorageService.GetSharedFiles(UserId);
            return Ok(storages);
        }
    }
}