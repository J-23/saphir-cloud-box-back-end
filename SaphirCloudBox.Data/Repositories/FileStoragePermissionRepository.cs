using Anthill.Common.Data;
using Microsoft.EntityFrameworkCore;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Repositories
{
    public class FileStoragePermissionRepository : AbstractRepository<User, Role, int>, IFileStoragePermissionRepository
    {
        public FileStoragePermissionRepository(SaphirCloudBoxDataContext context) : base(context)
        {
        }

        public async Task Add(FileStoragePermission permission)
        {
            await Context.Set<FileStoragePermission>()
                .AddAsync(permission);

            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FileStoragePermission>> GetByUserId(int userId)
        {
            return await Context.Set<FileStoragePermission>()
                .Where(x => x.RecipientId == userId && !x.EndDate.HasValue)
                .ToListAsync();
        }

        public async Task<bool> IsAvailable(int userId, PermissionType permissionType)
        {
            return await Context.Set<FileStoragePermission>()
                .AnyAsync(x => x.RecipientId == userId
                                && x.Type == permissionType
                                && !x.EndDate.HasValue);
        }
    }
}
