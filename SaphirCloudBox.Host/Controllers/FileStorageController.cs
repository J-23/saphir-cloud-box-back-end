using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Host.Helpers;
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
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        private readonly IEmailSender _emailSender;

        private readonly AppSettings _appSettings;

        public FileStorageController(IFileStorageService fileStorageService,
            INotificationService notificationService, 
            IUserService userService,
            ILogService logService,
            IEmailSender emailSender,
            AppSettings appSettings) : base(logService)
        {
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));

            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            MessageUtil.AppSettings = _appSettings;
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
        [Route("download/file/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var fileDto = await _fileStorageService.GetFileById(id, UserId, ClientId);

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
        [Route("check/permission")]
        public async Task<ActionResult> AddPermission([FromBody]CheckPermissionDto permissionDto)
        {
            if (!IsAvailableOperation() || permissionDto.UserIds.Count() == 0 && permissionDto.ClientIds.Count() == 0 && permissionDto.GroupIds.Count() == 0)
            {
                return BadRequest();
            }

            var result = await _fileStorageService.CheckPermission(permissionDto, UserId, ClientId);
            AddLog(LogType.Create, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.CreateVerb, UserId));

            foreach (var recipient in result.Recipients)
            {
                var message = MessageUtil.GetMessage(permissionDto.Type, recipient, result.Storage, result.Sender);

                try
                {
                    await _emailSender.Send(EmailType.Notification, new MailAddress(recipient.Email, recipient.UserName), message.Subject, message.MessageBody);
                    await _notificationService.Add(recipient.Id, result.Storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.Success);
                }
                catch (Exception)
                {
                    await _notificationService.Add(recipient.Id, result.Storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.NotSent);
                }
            }
            
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

            var result = await _fileStorageService.UpdatePermission(permissionDto, UserId, ClientId);
            AddLog(LogType.Update, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.UpdateVerb, UserId));

            var recipient = result.Recipients.First();
            var message = MessageUtil.GetUpdatePermissionMessage(permissionDto.Type, recipient, result.Storage, result.Sender);

            try
            {
                await _emailSender.Send(EmailType.Notification, new MailAddress(recipient.Email, recipient.UserName), message.Subject, message.MessageBody);
                await _notificationService.Add(recipient.Id, result.Storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.Success); 
            }
            catch (Exception)
            {
                await _notificationService.Add(recipient.Id, result.Storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.NotSent);
            }
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

            var result = await _fileStorageService.RemovePermission(permissionDto, UserId, ClientId);
            AddLog(LogType.Remove, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.UpdateVerb, UserId));

            var recipient = result.Recipients.First();
            var message = MessageUtil.GetRemovePermissionMessage(recipient, result.Storage, result.Sender);

            try
            {
                await _emailSender.Send(EmailType.Notification, new MailAddress(recipient.Email, recipient.UserName), message.Subject, message.MessageBody);
                await _notificationService.Add(recipient.Id, result.Storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.Success);
            }
            catch (Exception ex)
            {
                await _notificationService.Add(recipient.Id, result.Storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.NotSent);
            }

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

            var storages = await _fileStorageService.GetSharedFiles(UserId, ClientId);

            var response = new
            {
                Storages = storages.Storages,
                NewFileCount = storages.NewFileCount
            };

            return Ok(response);
        }
    }
}