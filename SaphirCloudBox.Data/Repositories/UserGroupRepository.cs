using Anthill.Common.Data;
using Microsoft.EntityFrameworkCore;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Repositories
{
    public class UserGroupRepository : AbstractRepository<User, Role, int>, IUserGroupRepository
    {
        public UserGroupRepository(SaphirCloudBoxDataContext context) : base(context)
        {
        }

        public async Task Add(Group group)
        {
            await Context.Set<Group>().AddAsync(group);
            await Context.SaveChangesAsync();
        }

        public async Task<Group> GetById(int groupId, int userId)
        {
            return await Context.Set<Group>()
                .FirstOrDefaultAsync(x => x.IsActive && x.Id == groupId && x.OwnerId == userId);
        }

        public async Task<IEnumerable<Group>> GetByIds(IEnumerable<int> groupIds)
        {
            return await Context.Set<Group>()
                .Where(x => groupIds.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<Group> GetByName(string groupName, int userId)
        {
            return await Context.Set<Group>()
                   .FirstOrDefaultAsync(x => x.IsActive && x.Name.Equals(groupName) && x.OwnerId == userId);
        }

        public async Task<IEnumerable<Group>> GetGroups(int userId)
        {
            return await Context.Set<Group>()
                .Where(x => x.IsActive && x.OwnerId == userId)
                .OrderBy(ord => ord.Name)
                .ToListAsync();
        }

        public async Task Update(Group group)
        {
            Context.Entry(group).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }
    }
}
