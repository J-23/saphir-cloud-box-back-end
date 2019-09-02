using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos.UserGroup
{
    public class UserGroupDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<UserDto> Users { get; set; }
    }
}
