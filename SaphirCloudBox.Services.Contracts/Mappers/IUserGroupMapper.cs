using Anthill.Common.Services.Contracts;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos.UserGroup;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Mappers
{
    public interface IUserGroupMapper : IMapper<Group, UserGroupDto>
    {
    }
}
