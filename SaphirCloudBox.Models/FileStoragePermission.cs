using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class FileStoragePermission
    {
        public int Id { get; set; }

        public int FileStorageId { get; set; }

        public int RecipientId { get; set; }

        public int SenderId { get; set; }

        public PermissionType Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public virtual FileStorage FileStorage { get; set; }

        public virtual User Sender { get; set; }

        public virtual User Recipient { get; set; }
    }
}
