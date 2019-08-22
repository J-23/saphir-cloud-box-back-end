using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Dtos.Permission;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Contracts.Services
{
    public interface ISharedFileService
    {
        Task<IEnumerable<SharedFileDto>> GetByUserId(int userId);

        Task AddPermission(AddPermissionDto permissionDto, int userId);
    }
}
