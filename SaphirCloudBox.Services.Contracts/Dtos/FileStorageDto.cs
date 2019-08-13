using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class FileStorageDto
    {
        public int? OwnerId { get; set; }

        public int? ClientId { get; set; }

        public IEnumerable<FileDto> Files { get; set; }
        public class FileDto
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public bool IsDirectory { get; set; }

            public UserDto CreateBy { get; set; }

            public UserDto UpdateBy { get; set; }

            public DateTime CreateDate { get; set; }

            public DateTime? UpdateDate { get; set; }
        }
    }
}
