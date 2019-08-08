using SaphirCloudBox.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Contracts.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAll();

        Task Add(AddRoleDto roleDto);

        Task Update(RoleDto roleDto);

        Task Remove(RemoveRoleDto roleDto);
    }
}
