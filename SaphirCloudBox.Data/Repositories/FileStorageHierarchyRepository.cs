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
    public class FileStorageHierarchyRepository : AbstractRepository<User, Role, int>, IFileStorageHierarchyRepository
    {
        private readonly SaphirUserManager _userManager;
        private readonly RoleManager<Role> _roleManager;

        public FileStorageHierarchyRepository(SaphirCloudBoxDataContext context, SaphirUserManager userManager, RoleManager<Role> roleManager) : base(context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task<IEnumerable<FileStorage>> GetByParentId(int parentId, int userId, int clientId)
        {
            if (parentId == 1)
            {
                return await Context.Set<FileStorage>()
                    .Where(x => x.ParentFileStorageId == 1
                            && x.IsDirectory
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
                            && x.IsDirectory
                            && x.IsActive
                            && ((!x.ClientId.HasValue && !x.OwnerId.HasValue && roles.Any(y => y.RoleType == RoleType.SuperAdmin))
                                || (x.ClientId.HasValue && !x.OwnerId.HasValue && x.ClientId.Value == clientId && roles.Any(y => y.RoleType == RoleType.ClientAdmin))
                                || (!x.ClientId.HasValue && x.OwnerId.HasValue && x.OwnerId.Value == userId)
                                || (x.Permissions.Any(y => y.RecipientId == userId && !y.EndDate.HasValue))))
                .OrderBy(ord => !ord.IsDirectory)
                .ThenBy(ord => ord.Name)
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
                    && !(x.CreateById == userId)
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
                    && !(x.CreateById == userId)
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
