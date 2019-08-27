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

            var storage = await _fileStorageService.AddPermission(permissionDto, UserId, ClientId);
            AddLog(Enums.LogType.Create, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.CreateVerb, UserId));

            var message = GetAddPermissionMessage(storage, permissionDto);

            try
            {
                await _emailSender.Send(EmailType.Notification, new MailAddress(message.Recipient.Email, message.Recipient.UserName), message.Subject, message.MessageBody);
                await _notificationService.Add(message.Recipient.Id, storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.Success);
            }
            catch (Exception)
            {
                await _notificationService.Add(message.Recipient.Id, storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.NotSent);
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

            var storage = await _fileStorageService.UpdatePermission(permissionDto, UserId, ClientId);
            AddLog(Enums.LogType.Update, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.UpdateVerb, UserId));

            var message = GetUpdatePermissionMessage(storage, permissionDto);

            try
            {
                await _emailSender.Send(EmailType.Notification, new MailAddress(message.Recipient.Email, message.Recipient.UserName), message.Subject, message.MessageBody);
                await _notificationService.Add(message.Recipient.Id, storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.Success); 
            }
            catch (Exception)
            {
                await _notificationService.Add(message.Recipient.Id, storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.NotSent);
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

            var storage = await _fileStorageService.RemovePermission(permissionDto, UserId, ClientId);
            AddLog(Enums.LogType.Remove, LogMessage.CreatePermissionMessage(permissionDto.FileStorageId, LogMessage.UpdateVerb, UserId));

            var message = GetRemovePermissionMessage(storage, permissionDto, UserId);

            try
            {
                await _emailSender.Send(EmailType.Notification, new MailAddress(message.Recipient.Email, message.Recipient.UserName), message.Subject, message.MessageBody);
                await _notificationService.Add(message.Recipient.Id, storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.Success);
            }
            catch (Exception)
            {
                await _notificationService.Add(message.Recipient.Id, storage.Id, message.Subject, message.MessageBody, Enums.NotificationType.NotSent);
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

            var storages = await _fileStorageService.GetSharedFiles(UserId);
            return Ok(storages);
        }

        private (string Subject, string MessageBody, UserDto Recipient) GetAddPermissionMessage(FileStorageDto.StorageDto storage, AddPermissionDto permissionDto)
        {
            var permission = storage.Permissions.FirstOrDefault(x => x.Type == permissionDto.Type && x.Recipient.Email.Equals(permissionDto.RecipientEmail));

            var fileStorageName = String.Join("", storage.Name, storage.File?.Extension);
            var parentStorageId = storage.Client == null && storage.Owner != null ? _appSettings.SharedWithMeUrlPart : storage.ParentStorageId.ToString();
            var fileStorageType = storage.IsDirectory ? Constants.NotificationMessages.Folder : Constants.NotificationMessages.File;

            var link = String.Join("/", _appSettings.FrontEndUrl, _appSettings.FileManagerUrlPart, parentStorageId);

            var subject = Constants.NotificationMessages.NotificationSubject;
            var message = String.Format(Constants.NotificationMessages.AddPermissionNotificationMessage, permission.Recipient.UserName, permission.Sender.UserName,
                permissionDto.Type.ToString(), fileStorageType, fileStorageName, link);

            return (subject, message, permission.Recipient);
        }

        private (string Subject, string MessageBody, UserDto Recipient) GetUpdatePermissionMessage(FileStorageDto.StorageDto storage, UpdatePermissionDto permissionDto)
        {
            var permission = storage.Permissions.FirstOrDefault(x => x.Type == permissionDto.Type && x.Recipient.Email.Equals(permissionDto.RecipientEmail));

            var fileStorageName = String.Join("", storage.Name, storage.File?.Extension);
            var parentStorageId = storage.Client == null && storage.Owner != null ? _appSettings.SharedWithMeUrlPart : storage.ParentStorageId.ToString();
            var fileStorageType = storage.IsDirectory ? Constants.NotificationMessages.Folder : Constants.NotificationMessages.File;

            var link = String.Join("/", _appSettings.FrontEndUrl, _appSettings.FileManagerUrlPart, parentStorageId);

            var subject = Constants.NotificationMessages.NotificationSubject;
            var message = String.Format(Constants.NotificationMessages.UpdatePermissionNotificationMessage, permission.Recipient.UserName, permission.Sender.UserName,
                fileStorageType, fileStorageName, permissionDto.Type.ToString(), link);

            return (subject, message, permission.Recipient);
        }

        private (string Subject, string MessageBody, UserDto Recipient) GetRemovePermissionMessage(FileStorageDto.StorageDto storage, RemovePermissionDto permissionDto, int userId)
        {
            var permission = storage.Permissions.FirstOrDefault(x => x.Recipient.Email.Equals(permissionDto.RecipientEmail) && x.Sender.Id == userId);

            var fileStorageName = String.Join("", storage.Name, storage.File?.Extension);
            var parentStorageId = storage.Client == null && storage.Owner != null ? _appSettings.SharedWithMeUrlPart : storage.ParentStorageId.ToString();
            var fileStorageType = storage.IsDirectory ? Constants.NotificationMessages.Folder : Constants.NotificationMessages.File;

            var subject = Constants.NotificationMessages.NotificationSubject;
            var message = String.Format(Constants.NotificationMessages.RemovePermissionNotificationMessage, permission.Recipient.UserName, permission.Sender.UserName,
                fileStorageType, fileStorageName);

            return (subject, message, permission.Recipient);
        }
    }
}