using SaphirCloudBox.Enums;
using SaphirCloudBox.Services.Contracts.Dtos.UserGroup;
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

        public PermissionInfoDto PermissionInfo { get; set; }

        public IEnumerable<StorageDto.PermissionDto> Permissions { get; set; }

        public class StorageDto
        {
            public int Id { get; set; }

            public int? ParentStorageId { get; set; }

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

            public PermissionInfoDto PermissionInfo { get; set; }

            public IEnumerable<PermissionDto> Permissions { get; set; }

            public bool IsViewed { get; set; }

            public class PermissionDto
            {
                public UserDto Recipient { get; set; }

                public PermissionType Type { get; set; }
            }
        }

        public class PermissionInfoDto
        {
            public IEnumerable<UserDto> Recipients { get; set; }

            public IEnumerable<ClientDto> Clients { get; set; }

            public IEnumerable<UserGroupDto> Groups { get; set; }

            public PermissionType Type { get; set; }
        }
    }
}
