using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class File
    {
        public int Id { get; set; }

        public int FileStorageId { get; set; }

        public string Extension { get; set; }

        public int Size { get; set; }

        public string SizeType { get; set; }

        public bool IsActive { get; set; }

        public int CreateById { get; set; }

        public DateTime CreateDate { get; set; }

        public int? UpdateById { get; set; }

        public DateTime? UpdateDate { get; set; }

        public virtual FileStorage FileStorage { get; set; }

        public virtual AzureBlobStorage AzureBlobStorage { get; set; }

        public virtual User CreateBy { get; set; }

        public virtual User UpdateBy { get; set; }
    }
}
