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
    public class LogRepository : AbstractRepository<User, Role, int>, ILogRepository
    {
        public LogRepository(SaphirCloudBoxDataContext context) : base(context)
        {
        }

        public void Add(Log log)
        {
            Context.Set<Log>().Add(log);
            Context.SaveChanges();
        }
    }
}
