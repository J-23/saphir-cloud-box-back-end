using Anthill.Common.Data.Contracts;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Contracts.Repositories
{
    public interface IFileStorageHierarchyRepository: IRepository
    {
        Task<IEnumerable<FileStorage>> GetByParentId(int parentId, int userId, int clientId);

        Task<int> GetNewFileCountByParentId(int id, int userId, int clientId);
    }
}
