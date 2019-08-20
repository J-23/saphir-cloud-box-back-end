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
    public class DepartmentRepository : AbstractRepository<User, Role, int>, IDepartmentRepository
    {
        public DepartmentRepository(SaphirCloudBoxDataContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Department>> GetAll()
        {
            return await Context.Set<Department>()
                .OrderByDescending(ord => ord.CreateDate)
                .ThenByDescending(ord => ord.UpdateDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Department>> GetByClientId(int clientId)
        {
            return await Context.Set<Department>()
                .Where(x => x.ClientId == clientId)
                .OrderByDescending(ord => ord.CreateDate)
                .ThenByDescending(ord => ord.UpdateDate)
                .ToListAsync();
        }

        public async Task<Department> GetById(int departmentId)
        {
            return await Context.Set<Department>().FirstOrDefaultAsync(x => x.Id == departmentId);
        }

        public async Task Remove(Department department)
        {
            Context.Set<Department>().Remove(department);
            await Context.SaveChangesAsync();
        }

        public async Task Update(Department department)
        {
            Context.Entry(department).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }
    }
}
