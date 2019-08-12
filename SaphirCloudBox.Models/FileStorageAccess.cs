using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class FileStorageAccess
    {
        public int FileStorageId { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
