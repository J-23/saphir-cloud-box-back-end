using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class RoleDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public RoleType RoleType { get; set; }
    }
}
