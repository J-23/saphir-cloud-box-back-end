using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class FileStorageDto
    {
        public UserDto Owner { get; set; }

        public ClientDto Client { get; set; }

        public int? ParentStorageId { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<StorageDto> Storages { get; set; }

        public IEnumerable<StorageDto.PermissionDto> Permissions { get; set; }

        public class StorageDto
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public bool IsDirectory { get; set; }

            public UserDto CreateBy { get; set; }

            public UserDto UpdateBy { get; set; }

            public DateTime CreateDate { get; set; }

            public DateTime? UpdateDate { get; set; }

            public UserDto Owner { get; set; }

            public ClientDto Client { get; set; }

            public string StorageType { get; set; }

            public FileDto File { get; set; }

            public IEnumerable<PermissionDto> Permissions { get; set; }

            public class PermissionDto
            {
                public virtual UserDto Recipient { get; set; }

                public PermissionType Type { get; set; }
            }
        }
    }
}
