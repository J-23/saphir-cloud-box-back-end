using Anthill.Common.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Repositories
{
    public class UserRepository : AbstractRepository<User, IdentityRole<int>, int>, IUserRepository
    {
        public UserRepository(SaphirCloudBoxDataContext context) : base(context)
        {
        }

        public async Task<IEnumerable<User>> GetUsersByClientId(int clientId)
        {
            return await Context.Set<User>()
                .Where(x => x.ClientId == clientId)
                .ToListAsync();
        }
    }
}
