using Anthill.Common.Data;
using Microsoft.AspNetCore.Identity;
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
    public class FileStorageRepository : AbstractRepository<User, Role, int>, IFileStorageRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public FileStorageRepository(SaphirCloudBoxDataContext context, UserManager<User> userManager, RoleManager<Role> roleManager) : base(context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
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
                    || (!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)
                    || (!x.OwnerId.HasValue && x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue))));
        }

        public async Task<IEnumerable<FileStorage>> GetByParentId(int parentId, int userId, int clientId)
        {
            return await Context.Set<FileStorage>()
                .Where(x => x.ParentFileStorageId == parentId 
                && ((!x.ClientId.HasValue && !x.OwnerId.HasValue)
                    || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId)
                    || (!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)
                    || (!x.OwnerId.HasValue && x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue))))
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

        public async Task<Boolean> IsAvailableToChange(int id, int userId, int clientId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.Where(x => roleNames.Contains(x.Name)).ToListAsync();

            var fileStorage = await Context.Set<FileStorage>()
                .FirstOrDefaultAsync(x => x.Id == id && ((x.Owner == null && x.Client == null && roles.Any(y => y.RoleType == RoleType.SuperAdmin))
                            || (x.Owner == null && x.Client != null && roles.Any(y => y.RoleType == RoleType.ClientAdmin) && x.ClientId == clientId)
                            || (x.Owner != null && x.Client == null && (roles.Any(y => y.RoleType == RoleType.DepartmentHead)
                            || roles.Any(y => y.RoleType == RoleType.Employee) || x.OwnerId == userId))));

            if (fileStorage == null)
            {
                return false;
            }

            return await IsAvailableToChange(id, userId, roles, clientId);
        }

        private async Task<bool> IsAvailableToChange(int id, int userId, IEnumerable<Role> roles, int clientId)
        {
            var fileStorages = await Context.Set<FileStorage>()
                .Where(x => x.ParentFileStorageId.HasValue && x.ParentFileStorageId.Value == id)
                .ToListAsync();

            if (fileStorages.Any(x => !((x.Owner == null && x.Client == null && roles.Any(y => y.RoleType == RoleType.SuperAdmin))
                            || (x.Owner == null && x.Client != null && roles.Any(y => y.RoleType == RoleType.ClientAdmin) && x.ClientId == clientId)
                            || (x.Owner != null && x.Client == null && (roles.Any(y => y.RoleType == RoleType.DepartmentHead)
                            || roles.Any(y => y.RoleType == RoleType.Employee) || x.OwnerId == userId)))))
            {
                return false;
            }

            foreach (var fileStorage in fileStorages)
            {
                return await IsAvailableToChange(fileStorage.Id, userId, roles, clientId);
            }

            return true;
        }

        public async Task<IEnumerable<FileStorage>> GetFilesByParentId(int parentId, int userId, int clientId)
        {
            var fileStorages = await Context.Set<FileStorage>()
                .Where(x => x.ParentFileStorageId.HasValue && parentId == x.ParentFileStorageId.Value
                    && ((!x.ClientId.HasValue && !x.OwnerId.HasValue)
                        || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId)
                        || (!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)))
                .ToListAsync();

            var files = fileStorages.Where(x => !x.IsDirectory).ToList();
            var folders = fileStorages.Where(x => x.IsDirectory).ToList();

            foreach (var folder in folders)
            {
                var childFiles = await GetFilesByParentId(folder.Id, userId, clientId);

                files.AddRange(childFiles);
            }

            return files;
        }
    }
}
