using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos.Permission
{
    public class UpdatePermissionDto
    {
        [Required]
        public int RecipientId { get; set; }

        [Required]
        public int FileStorageId { get; set; }

        [Required]
        public PermissionType Type { get; set; }
    }
}
