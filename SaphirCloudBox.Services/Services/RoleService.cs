using Anthill.Common.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaphirCloudBox.Data;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Mappers;
using SaphirCloudBox.Services.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class RoleService : AbstractService, IRoleService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly SaphirUserManager _userManager;

        public RoleService(IUnityContainer container,
            ISaphirCloudBoxDataContextManager dataContextManager, 
            RoleManager<Role> roleManager,
            SaphirUserManager userManager) : base(container, dataContextManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task Add(AddRoleDto roleDto)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name.Equals(roleDto.Name) && x.IsActive);

            if (role != null)
            {
                throw new FoundSameObjectException("Role", roleDto.Name);
            }

            role = new Role {
                Name = roleDto.Name,
                RoleType = Enums.RoleType.Employee,
                IsActive = true
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                throw new RoleManagerException("add", role.Name);
            }
        }

        public async Task<IEnumerable<RoleDto>> GetAll()
        {
            var roles = await _roleManager.Roles.Where(x => x.IsActive).OrderBy(ord => ord.Name).ToListAsync();
            return MapperFactory.CreateMapper<IRoleMapper>().MapCollectionToModel(roles);
        }

        public async Task Remove(RemoveRoleDto roleDto)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleDto.Id && x.IsActive);

            if (role == null)
            {
                throw new NotFoundException("Role", roleDto.Id);
            }


            var users = await _userManager.GetUsersInRoleAsync(role.Name);

            if (users.Where(x => x.IsActive).Count() > 0)
            {
                throw new ExistDependencyException("Role", role.Id, new List<string> { "Users" });
            }

            role.IsActive = false;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                throw new RoleManagerException("remove", role.Name);
            }
        }

        public async Task Update(RoleDto roleDto)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleDto.Id && x.IsActive);

            if (role == null)
            {
                throw new NotFoundException("Role", roleDto.Id);
            }

            var otherRole = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name.Equals(roleDto.Name) && x.IsActive);

            if (otherRole != null && otherRole.Id != role.Id)
            {
                throw new FoundSameObjectException("Role", roleDto.Name);
            }

            role.Name = roleDto.Name;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                throw new RoleManagerException("update", role.Name);
            }
        }
    }
}
