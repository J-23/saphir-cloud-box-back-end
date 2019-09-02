using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos.UserGroup
{
    public class AddUserGroupDto
    {
        public string Name { get; set; }

        public IEnumerable<int> UserIds { get; set; }
    }
}
