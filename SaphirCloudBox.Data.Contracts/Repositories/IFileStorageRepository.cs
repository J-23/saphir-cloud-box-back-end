using Anthill.Common.Data.Contracts;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Contracts.Repositories
{
    public interface IFileStorageRepository: IRepository
    {
        Task<IEnumerable<FileStorage>> GetByParentId(int parentId, int userId, int clientId);

        Task<FileStorage> GetById(int id, int userId, int clientId);

        Task Add(FileStorage fileStorage);

        Task Update(FileStorage fileStorage);

        Task<Boolean> UserHasFolder(int id);

        Task<Boolean> IsAvailableToChange(int id, int userId, int clientId);

        Task<IEnumerable<FileStorage>> GetFilesByParentId(int id, int userId, int clientId);

        Task<IEnumerable<FileStorage>> GetSharedFiles(int userId);

        Task<IEnumerable<FileStorage>> GetParents(int? parentId, int userId, int clientId);

        Task Update(IEnumerable<FileStorage> fileStorages);

        Task<IEnumerable<FileStorage>> GetByUserId(int id);
    }
}
