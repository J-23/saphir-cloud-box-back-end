using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class FolderDto
    {
        public int Id { get; set; }

        public int Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public IEnumerable<FileDto> Files { get; set; }
    }
}
