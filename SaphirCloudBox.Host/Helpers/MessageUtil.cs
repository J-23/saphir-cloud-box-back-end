using SaphirCloudBox.Enums;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Dtos.Permission;
using SaphirCloudBox.Host.Infractructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Helpers
{
    public static class MessageUtil
    {
        public static AppSettings AppSettings { get; set; }

        public static (string Subject, string MessageBody) GetMessage(PermissionType type, CheckPermissionResultDto.RecipientDto recipient,
            FileStorageDto.StorageDto storage, UserDto sender)
        {
            if (recipient.Type == CheckPermissionResultDto.UpdateType.Add)
            {
                return GetAddPermissionMessage(type, recipient, storage, sender);
            }
            else if (recipient.Type == CheckPermissionResultDto.UpdateType.Update)
            {
                return GetUpdatePermissionMessage(type, recipient, storage, sender);
            }
            else if (recipient.Type == CheckPermissionResultDto.UpdateType.Remove)
            {
                return GetRemovePermissionMessage(recipient, storage, sender);
            }
            else
            {
                throw new ArgumentNullException(nameof(recipient.Type));
            }
        }

        public static (string Subject, string MessageBody) GetAddPermissionMessage(PermissionType type, CheckPermissionResultDto.RecipientDto recipient,
            FileStorageDto.StorageDto storage, UserDto sender)
        {
            var parentStorageId = storage.Client == null && storage.Owner != null ? AppSettings.SharedWithMeUrlPart : storage.ParentStorageId.Value.ToString();
            var link = String.Join("/", AppSettings.FrontEndUrl, AppSettings.FileManagerUrlPart, parentStorageId);

            var fileStorageType = storage.IsDirectory ? Constants.NotificationMessages.Folder : Constants.NotificationMessages.File;
            var fileStorageName = storage.Name + storage.File?.Extension;

            var subject = Constants.NotificationMessages.NotificationSubject;
            var message = String.Format(Constants.NotificationMessages.AddPermissionNotificationMessage, recipient.UserName, sender.UserName,
                type.ToString(), fileStorageType, fileStorageName, link);

            return (subject, message);
        }

        public static (string Subject, string MessageBody) GetUpdatePermissionMessage(PermissionType type, CheckPermissionResultDto.RecipientDto recipient, 
            FileStorageDto.StorageDto storage, UserDto sender)
        {
            var parentStorageId = storage.Client == null && storage.Owner != null ? AppSettings.SharedWithMeUrlPart : storage.ParentStorageId.ToString();
            var link = String.Join("/", AppSettings.FrontEndUrl, AppSettings.FileManagerUrlPart, parentStorageId);

            var fileStorageType = storage.IsDirectory ? Constants.NotificationMessages.Folder : Constants.NotificationMessages.File;
            var fileStorageName = String.Join("", storage.Name, storage.File?.Extension);

            var subject = Constants.NotificationMessages.NotificationSubject;
            var message = String.Format(Constants.NotificationMessages.UpdatePermissionNotificationMessage, recipient.UserName, sender.UserName,
                fileStorageType, fileStorageName, type.ToString(), link);

            return (subject, message);
        }

        public static (string Subject, string MessageBody) GetRemovePermissionMessage(CheckPermissionResultDto.RecipientDto recipient, FileStorageDto.StorageDto storage, UserDto sender)
        {
            var fileStorageName = String.Join("", storage.Name, storage.File?.Extension);
            var fileStorageType = storage.IsDirectory ? Constants.NotificationMessages.Folder : Constants.NotificationMessages.File;

            var subject = Constants.NotificationMessages.NotificationSubject;
            var message = String.Format(Constants.NotificationMessages.RemovePermissionNotificationMessage, recipient.UserName, sender.UserName,
                fileStorageType, fileStorageName);

            return (subject, message);
        }
    }
}
