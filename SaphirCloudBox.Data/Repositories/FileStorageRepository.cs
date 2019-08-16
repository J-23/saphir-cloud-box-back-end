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

        public async Task<FileStorage> GetById(int id, int userId, int clientId)
        {
            return await Context.Set<FileStorage>()
                .FirstOrDefaultAsync(x => x.Id == id && ((!x.ClientId.HasValue && !x.OwnerId.HasValue)
                    || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId)
                    || (!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)));
        }

        public async Task<IEnumerable<FileStorage>> GetByParentId(int parentId, int userId, int clientId)
        {
            return await Context.Set<FileStorage>()
                .Where(x => x.ParentFileStorageId == parentId 
                && ((!x.ClientId.HasValue && !x.OwnerId.HasValue)
                    || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId)
                    || (!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)))
                .OrderBy(ord => !ord.IsDirectory)
                .ThenBy(ord => ord.Name)
                .ToListAsync();
        }

        public async Task Remove(FileStorage fileStorage)
        {
            Context.Set<FileStorage>().Remove(fileStorage);
            await Context.SaveChangesAsync();
        }

        public async Task RemoveFolder(FileStorage fileStorage)
        {
            var fileStorages = await Context.Set<FileStorage>()
                    .Where(x => x.ParentFileStorageId.HasValue && x.ParentFileStorageId.Value == fileStorage.Id)
                    .ToListAsync();

            foreach (var storage in fileStorages)
            {
                await RemoveChildFileStorages(storage);
            }

            Context.Set<FileStorage>().Remove(fileStorage);
            await Context.SaveChangesAsync();
        }

        private async Task RemoveChildFileStorages(FileStorage fileStorage)
        {
            var fileStorages = await Context.Set<FileStorage>()
                    .Where(x => x.ParentFileStorageId.HasValue && x.ParentFileStorageId.Value == fileStorage.Id)
                    .ToListAsync();

            foreach (var storage in fileStorages)
            {
                await RemoveChildFileStorages(storage);
            }

            Context.Set<FileStorage>().Remove(fileStorage);
        }

        public async Task Update(FileStorage fileStorage)
        {
            Context.Entry(fileStorage).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }

        public async Task<bool> UserHasFolder(int id)
        {
            return await Context.Set<FileStorage>()
                .AnyAsync(x => x.ParentFileStorageId == 1 && x.OwnerId == id);
        }

        public async Task<IEnumerable<FileStorage>> GetAllByParentId(int id)
        {
            var fileStorages = await Context.Set<FileStorage>()
                .Where(x => x.ParentFileStorageId.HasValue && x.ParentFileStorageId.Value == id)
                .ToListAsync();

            var childFileStorages = new List<FileStorage>();

            foreach (var fileStorage in fileStorages)
            {
                childFileStorages.AddRange(await GetAllByParentId(fileStorage.Id));
            }

            fileStorages.AddRange(childFileStorages);

            return fileStorages;
        }
    }
}
