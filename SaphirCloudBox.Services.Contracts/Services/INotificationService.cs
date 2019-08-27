using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Contracts.Services
{
    public interface INotificationService
    {
        Task Add(int userId, int fileStorageId, string subject, string message, NotificationType type);

        Task Add(int userId, string subject, string message, NotificationType type);

        Task Add(string subject, string message, NotificationType type);
    }
}
