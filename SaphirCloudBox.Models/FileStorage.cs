using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class FileStorage
    {
        public int Id { get; set; }

        public Boolean IsDirectory { get; set; }

        public string Name { get; set; }

        public Guid? BlobName { get; set; }

        public int? ClientId { get; set; }

        public int? OwnerId { get; set; }

        public int CreateById { get; set; }

        public DateTime CreateDate { get; set; }

        public int? UpdateById { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? ParentFileStorageId { get; set; }

        public virtual Client Client { get; set; }

        public virtual User Owner { get; set; }

        public virtual FileStorage ParentFileStorage { get; set; }

        public virtual User CreateBy { get; set; }

        public virtual User UpdateBy { get; set; }

        public virtual IEnumerable<FileStorageAccessUser> FileStorageAccessUsers { get; set; }
    }
}
