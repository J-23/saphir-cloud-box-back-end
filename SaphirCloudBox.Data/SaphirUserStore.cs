using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data
{
    public class SaphirUserStore : UserStore<User, Role, SaphirCloudBoxDataContext, int>
    {
        public SaphirUserStore(SaphirCloudBoxDataContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public async Task<IEnumerable<User>> FindByClientIds(IEnumerable<int> clientIds)
        {
            return await Context.Set<User>()
                .Where(x => clientIds.Contains(x.ClientId))
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> FindByIds(IEnumerable<int> userIds)
        {
            return await Context.Set<User>()
                 .Where(x => userIds.Contains(x.Id))
                 .ToListAsync();
        }
    }
}
