using SaphirCloudBox.Services.Contracts.Dtos.UserGroup;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Contracts.Services
{
    public interface IUserGroupService
    {
        Task<IEnumerable<UserGroupDto>> GetGroups(int userId);

        Task<int> Add(AddUserGroupDto groupDto, int userId);

        Task Update(UpdateUserGroupDto groupDto, int userId);

        Task Remove(RemoveUserGroupDto groupDto, int userId);

        Task<UserGroupDto> GetById(int groupId, int userId);
    }
}
