using Anthill.Common.Data.Contracts;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Contracts.Repositories
{
    public interface IFileStoragePermissionRepository: IRepository
    {
        Task<IEnumerable<FileStoragePermission>> GetByUserId(int userId);

        Task<Boolean> IsAvailable(int userId, PermissionType permissionType);

        Task Add(FileStoragePermission permission);
    }
}
