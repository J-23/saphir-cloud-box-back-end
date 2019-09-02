using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos.UserGroup
{
    public class UpdateUserGroupDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<int> UserIds { get; set; }
    }
}
