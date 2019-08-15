using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class AzureBlobStorage
    {
        public int Id { get; set; }

        public Guid BlobName { get; set; }

        public int FileId { get; set; }

        public virtual File File { get; set; }
    }
}
