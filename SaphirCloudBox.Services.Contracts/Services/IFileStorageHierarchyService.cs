using SaphirCloudBox.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Contracts.Services
{
    public interface IFileStorageHierarchyService
    {
        Task<IEnumerable<FolderDto>> GetByParentId(int parentId, int userId, int clientId);
    }
}
