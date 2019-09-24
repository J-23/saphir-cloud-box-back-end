using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class FileViewing
    {
        public int Id { get; set; }

        public DateTime ViewDate { get; set; }

        public int ViewById { get; set; }

        public int FileStorageId { get; set; }

        public bool IsActive { get; set; }

        public virtual User ViewBy { get; set; }

        public virtual FileStorage FileStorage { get; set; }
    }
}
