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
    public class FileStorageRepository : AbstractRepository<User, IdentityRole<int>, int>, IFileStorageRepository
    {
        public FileStorageRepository(SaphirCloudBoxDataContext context) : base(context)
        {
        }

        public async Task Add(FileStorage fileStorage)
        {
            await Context.Set<FileStorage>().AddAsync(fileStorage);
            await Context.SaveChangesAsync();
        }

        public async Task<FileStorage> GetById(int id, int userId)
        {
            return await Context.Set<FileStorage>()
                .FirstOrDefaultAsync(x => x.Id == id && x.FileStorageAccesses.Any(y => y.UserId == userId));
        }

        public async Task<IEnumerable<FileStorage>> GetByParentId(int parentId, int userId)
        {
            return await Context.Set<FileStorage>()
                .Where(x => x.ParentFileStorageId == parentId && x.FileStorageAccesses.Any(y => y.UserId == userId))
                .ToListAsync();
        }

        public async Task Remove(FileStorage fileStorage)
        {
            Context.Set<FileStorage>().Remove(fileStorage);
            await Context.SaveChangesAsync();
        }

        public async Task Update(FileStorage fileStorage)
        {
            Context.Entry(fileStorage).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }
    }
}
