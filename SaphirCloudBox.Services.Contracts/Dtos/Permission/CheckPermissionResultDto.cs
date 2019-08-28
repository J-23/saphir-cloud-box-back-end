using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos.Permission
{
    public class CheckPermissionResultDto
    {
        public FileStorageDto.StorageDto Storage { get; set; }

        public UserDto Sender { get; set; }

        public IEnumerable<RecipientDto> Recipients { get; set; }

        public class RecipientDto
        {
            public int Id { get; set; }

            public string UserName { get; set; }

            public string Email { get; set; }

            public UpdateType Type { get; set; }
        }

        public enum UpdateType
        {
            Add,
            Update,
            Remove
        }
    }
}
