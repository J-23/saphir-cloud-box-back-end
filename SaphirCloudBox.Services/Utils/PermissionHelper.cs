using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Utils
{
    public class PermissionHelper: IPermissionHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public PermissionHelper(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task<bool> IsAvailableToChange(FileStorage fileStorage, IEnumerable<FileStorage> childFileStorages, int userId, int clientId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.Where(x => roleNames.Contains(x.Name)).ToListAsync();

            if (!((fileStorage.Owner == null && fileStorage.Client == null && roles.Any(x => x.RoleType == RoleType.SuperAdmin))
                                || (fileStorage.Owner == null && fileStorage.Client != null && roles.Any(x => x.RoleType == RoleType.ClientAdmin)
                                        && fileStorage.ClientId == clientId)
                                || (fileStorage.Owner != null && fileStorage.Client == null && (roles.Any(x => x.RoleType == RoleType.DepartmentHead)
                                        || roles.Any(x => x.RoleType == RoleType.Employee) || fileStorage.OwnerId == userId))))
            {
                return false;
            }

            foreach (var childFileStorage in childFileStorages)
            {
                if (!((fileStorage.Owner == null && fileStorage.Client == null && roles.Any(x => x.RoleType == RoleType.SuperAdmin))
                                || (fileStorage.Owner == null && fileStorage.Client != null && roles.Any(x => x.RoleType == RoleType.ClientAdmin)
                                        && fileStorage.ClientId == clientId)
                                || (fileStorage.Owner != null && fileStorage.Client == null && (roles.Any(x => x.RoleType == RoleType.DepartmentHead)
                                        || roles.Any(x => x.RoleType == RoleType.Employee) || fileStorage.OwnerId == userId))))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<(int? OwnerId, int? ClientId)> GetOwners(FileStorage parentFileStorage, int userId, int userClientId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.Where(x => roleNames.Contains(x.Name)).ToListAsync();

            int? ownerId = null;
            int? clientId = null;

            if (parentFileStorage.Id == 1)
            {
                foreach (var role in roles)
                {
                    if (role.RoleType == RoleType.SuperAdmin)
                    {
                        ownerId = null;
                        clientId = null;
                    }
                    else if (role.RoleType == RoleType.ClientAdmin)
                    {
                        ownerId = null;
                        clientId = userClientId;
                    }
                    else if (role.RoleType == RoleType.DepartmentHead || role.RoleType == RoleType.Employee)
                    {
                        ownerId = userId;
                        clientId = null;
                    }
                    else
                    {
                        ownerId = userId;
                        clientId = null;
                    }
                }
            }
            else
            {
                ownerId = parentFileStorage.OwnerId;
                clientId = parentFileStorage.ClientId;
            }

            return (ownerId, clientId);
        }
    }
}
