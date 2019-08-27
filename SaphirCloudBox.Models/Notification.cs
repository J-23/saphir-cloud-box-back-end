using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? FileStorageId { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public DateTime CreateDate { get; set; }

        public NotificationType Type { get; set; }

        public virtual FileStorage FileStorage { get; set; }

        public virtual User User { get; set; }
    }
}
