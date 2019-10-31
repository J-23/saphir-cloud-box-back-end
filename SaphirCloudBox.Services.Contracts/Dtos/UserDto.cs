using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public ClientDto Client { get; set; }

        public DepartmentDto Department { get; set; }

        public RoleDto Role { get; set; }

        public IEnumerable<int> GroupIds { get; set; }
    }
}
