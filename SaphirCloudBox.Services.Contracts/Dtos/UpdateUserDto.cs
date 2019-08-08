using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class UpdateUserDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public int ClientId { get; set; }

        public int? DepartmentId { get; set; }

        public int RoleId { get; set; }
    }
}
