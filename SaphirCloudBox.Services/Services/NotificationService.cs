using Anthill.Common.Data.Contracts;
using Anthill.Common.Services;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class NotificationService : AbstractService, INotificationService
    {
        public NotificationService(IUnityContainer container, ISaphirCloudBoxDataContextManager dataContextManager) 
            : base(container, dataContextManager)
        {
        }

        public async Task Add(int userId, int fileStorageId, string subject, string message, NotificationType type)
        {
            var notificationRepository = DataContextManager.CreateRepository<INotificationRepository>();

            var notification = new Notification
            {
                FileStorageId = fileStorageId,
                UserId = userId,
                Subject = subject,
                Message = message,
                CreateDate = DateTime.UtcNow,
                Type = type
            };

            await notificationRepository.Add(notification);
        }

        public async Task Add(int userId, string subject, string message, NotificationType type)
        {
            var notificationRepository = DataContextManager.CreateRepository<INotificationRepository>();

            var notification = new Notification
            {
                UserId = userId,
                Subject = subject,
                Message = message,
                CreateDate = DateTime.UtcNow,
                Type = type
            };

            await notificationRepository.Add(notification);
        }

        public async Task Add(string subject, string message, NotificationType type)
        {
            var notificationRepository = DataContextManager.CreateRepository<INotificationRepository>();

            var notification = new Notification
            {
                Subject = subject,
                Message = message,
                CreateDate = DateTime.UtcNow,
                Type = type
            };

            await notificationRepository.Add(notification);
        }
    }
}
