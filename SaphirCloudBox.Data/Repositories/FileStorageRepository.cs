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
        private readonly SaphirUserManager _userManager;
        private readonly RoleManager<Role> _roleManager;

        public FileStorageRepository(SaphirCloudBoxDataContext context, SaphirUserManager userManager, RoleManager<Role> roleManager) : base(context)
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
            if (id == 1)
            {
                return await Context.Set<FileStorage>()
                    .FirstOrDefaultAsync(x => x.Id == 1 && x.IsActive && ((!x.ClientId.HasValue && !x.OwnerId.HasValue)
                        || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId)
                        || (!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)));
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.Where(x => roleNames.Contains(x.Name)).ToListAsync();

            return await Context.Set<FileStorage>()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive && ((!x.ClientId.HasValue && !x.OwnerId.HasValue && (roles.Any(y => y.RoleType == RoleType.SuperAdmin) || x.ParentFileStorageId.HasValue && x.ParentFileStorageId.Value == 1))
                    || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId && (roles.Any(y => y.RoleType == RoleType.ClientAdmin) || x.ParentFileStorageId.HasValue && x.ParentFileStorageId.Value == 1))
                    || (!x.ClientId.HasValue && x.OwnerId.HasValue && (x.OwnerId.Value == userId || x.ParentFileStorageId.HasValue && x.ParentFileStorageId.Value == 1))
                    || (x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue))));
        }

        public async Task<IEnumerable<FileStorage>> GetByParentId(int parentId, int userId, int clientId)
        {
            if (parentId == 1)
            {
                return await Context.Set<FileStorage>()
                    .Where(x => x.ParentFileStorageId == 1
                            && x.IsActive
                            && ((!x.ClientId.HasValue && !x.OwnerId.HasValue)
                                || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId)
                                || (!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)
                                || (!x.OwnerId.HasValue && x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue))))
                    .OrderBy(ord => !ord.IsDirectory)
                    .ThenBy(ord => ord.Name)
                    .ToListAsync();
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.Where(x => roleNames.Contains(x.Name)).ToListAsync();

            return await Context.Set<FileStorage>()
                .Where(x => x.ParentFileStorageId == parentId 
                && x.IsActive
                && ((!x.ClientId.HasValue && !x.OwnerId.HasValue && roles.Any(y => y.RoleType == RoleType.SuperAdmin))
                    || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId && roles.Any(y => y.RoleType == RoleType.ClientAdmin))
                    || (!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)
                    || (x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue))))
                .OrderBy(ord => !ord.IsDirectory)
                .ThenBy(ord => ord.Name)
                .ToListAsync();
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
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive && ((x.Owner == null && x.Client == null && roles.Any(y => y.RoleType == RoleType.SuperAdmin))
                            || (x.Owner == null && x.Client != null && roles.Any(y => y.RoleType == RoleType.ClientAdmin) && x.ClientId == clientId)
                            || (x.Owner != null && x.Client == null && (roles.Any(y => y.RoleType == RoleType.DepartmentHead)
                            || roles.Any(y => y.RoleType == RoleType.Employee) || x.OwnerId == userId))
                            || (x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue && y.Type == PermissionType.ReadAndWrite))));

            if (fileStorage == null)
            {
                return false;
            }

            return await IsAvailableToChange(id, userId, roles, clientId);
        }

        private async Task<bool> IsAvailableToChange(int id, int userId, IEnumerable<Role> roles, int clientId)
        {
            var fileStorages = await Context.Set<FileStorage>()
                .Where(x => x.ParentFileStorageId.HasValue && x.ParentFileStorageId.Value == id && x.IsActive)
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

        public async Task<IEnumerable<FileStorage>> GetSharedFiles(int userId)
        {
            return await Context.Set<FileStorage>()
                .Where(x => !x.ClientId.HasValue && x.OwnerId.HasValue
                            && x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue))
                .ToListAsync();
        }

        public async Task<IEnumerable<FileStorage>> GetParents(int? parentId, int userId, int clientId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.Where(x => roleNames.Contains(x.Name)).ToListAsync();
            var parents = new List<FileStorage>();

            if (parentId.HasValue && parentId.Value != 1)
            {
                var parent = await Context.Set<FileStorage>()
                    .FirstOrDefaultAsync(x => x.IsDirectory
                                            && x.ParentFileStorageId != 1
                                            && x.IsActive
                                            && x.Id == parentId.Value 
                                            && ((!x.ClientId.HasValue && !x.OwnerId.HasValue && roles.Any(y => y.RoleType == RoleType.SuperAdmin))
                                            || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId && roles.Any(y => y.RoleType == RoleType.ClientAdmin))
                                            || (x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue))));

                if (parent != null)
                {
                    parents.Add(parent);

                    parents.AddRange(await GetParents(parent.ParentFileStorageId, userId, roles, clientId));
                }
                
            }

            return parents;
        }

        private async Task<IEnumerable<FileStorage>> GetParents(int? parentId, int userId, List<Role> roles, int clientId)
        {
            var parents = new List<FileStorage>();

            if (parentId.HasValue && parentId.Value != 1)
            {
                var parent = await Context.Set<FileStorage>()
                    .FirstOrDefaultAsync(x => x.IsDirectory
                                            && x.ParentFileStorageId != 1
                                            && x.Id == parentId.Value
                                            && x.IsActive
                                            && ((!x.ClientId.HasValue && !x.OwnerId.HasValue && roles.Any(y => y.RoleType == RoleType.SuperAdmin))
                                            || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId && roles.Any(y => y.RoleType == RoleType.ClientAdmin))
                                            || (x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue))));

                if (parent != null)
                {
                    parents.Add(parent);

                    parents.AddRange(await GetParents(parent.ParentFileStorageId, userId, roles, clientId));
                }
            }

            return parents;
        }

        public async Task Update(IEnumerable<FileStorage> fileStorages)
        {
            foreach (var fileStorage in fileStorages)
            {
                Context.Entry(fileStorage).State = EntityState.Modified;
            }

            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FileStorage>> GetByUserId(int id)
        {
            return await Context.Set<FileStorage>()
                .Where(x => !x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == id)
                .ToListAsync();
        }

        public async Task<int> GetNewFileCountByParentId(int fileStorageId, int userId, int clientId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.Where(x => roleNames.Contains(x.Name)).ToListAsync();

            var fileStorages = await Context.Set<FileStorage>()
                .Where(x => x.IsActive && x.ParentFileStorageId == fileStorageId
                        && ((!x.ClientId.HasValue && !x.OwnerId.HasValue && roles.Any(y => y.RoleType == RoleType.SuperAdmin))
                        || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId && roles.Any(y => y.RoleType == RoleType.ClientAdmin))
                        || (!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)
                        || (x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue))))
                .ToListAsync();

            var newFileCount = fileStorages
                .Where(x => !x.IsDirectory && (
                    !x.FileViewings.Any(y => y.ViewById == userId && y.IsActive)
                    && !(!x.ClientId.HasValue && !x.OwnerId.HasValue && roles.Any(y => y.RoleType == RoleType.SuperAdmin))
                    && !(x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId && roles.Any(y => y.RoleType == RoleType.ClientAdmin))
                    && !(!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)
                ))
                .Count();

            foreach (var storage in fileStorages.Where(x => x.IsDirectory).ToList())
            {
                newFileCount += await GetNewFileCountByParentId(storage.Id, userId, clientId, roles);
            }

            return newFileCount;
        }

        private async Task<int> GetNewFileCountByParentId(int fileStorageId, int userId, int clientId, IEnumerable<Role> roles)
        {
            var fileStorages = await Context.Set<FileStorage>()
                 .Where(x => x.IsActive && x.ParentFileStorageId == fileStorageId
                         && ((!x.ClientId.HasValue && !x.OwnerId.HasValue && roles.Any(y => y.RoleType == RoleType.SuperAdmin))
                         || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId && roles.Any(y => y.RoleType == RoleType.ClientAdmin))
                         || (!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)
                         || (x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue))))
                 .ToListAsync();

            var newFileCount = fileStorages
                .Where(x => !x.IsDirectory && (
                    !x.FileViewings.Any(y => y.ViewById == userId && y.IsActive)
                    && !(!x.ClientId.HasValue && !x.OwnerId.HasValue && roles.Any(y => y.RoleType == RoleType.SuperAdmin))
                    && !(x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId && roles.Any(y => y.RoleType == RoleType.ClientAdmin))
                    && !(!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)
                ))
                .Count();

            foreach (var storage in fileStorages.Where(x => x.IsDirectory).ToList())
            {
                newFileCount += await GetNewFileCountByParentId(storage.Id, userId, clientId, roles);
            }

            return newFileCount;
        }
    }
}
