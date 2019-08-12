using Anthill.Common.Data;
using Microsoft.AspNetCore.Identity;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Repositories
{
    public class LogRepository : AbstractRepository<User, IdentityRole<int>, int>, ILogRepository
    {
        public LogRepository(SaphirCloudBoxDataContext context) : base(context)
        {
        }

        public async Task Add(Log log)
        {
            await Context.Set<Log>().AddAsync(log);
            await Context.SaveChangesAsync();
        }
    }
}
