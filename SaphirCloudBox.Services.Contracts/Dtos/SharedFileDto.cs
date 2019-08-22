using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class SharedFileDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentStorageId { get; set; }

        public bool IsDirectory { get; set; }

        public PermissionType PermissionType { get; set; }

        public UserDto CreateBy { get; set; }

        public UserDto UpdateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public UserDto Owner { get; set; }

        public ClientDto Client { get; set; }

        public string StorageType { get; set; }

        public FileDto File { get; set; }
    }
}
