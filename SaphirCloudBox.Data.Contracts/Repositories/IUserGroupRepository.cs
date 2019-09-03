using Anthill.Common.Data.Contracts;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Contracts.Repositories
{
    public interface IUserGroupRepository: IRepository
    {
        Task<IEnumerable<Group>> GetGroups(int userId);

        Task Add(Group group);

        Task Update(Group group);

        Task<Group> GetById(int groupId, int userId);

        Task<Group> GetByName(string groupName, int userId);

        Task<IEnumerable<Group>> GetByIds(IEnumerable<int> groupIds);
    }
}
