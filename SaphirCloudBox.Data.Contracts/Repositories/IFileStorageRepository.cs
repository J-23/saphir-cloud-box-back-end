using Anthill.Common.Data.Contracts;
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

        Task Remove(FileStorage fileStorage);

        Task<Boolean> UserHasFolder(int id);

        Task RemoveFolder(FileStorage fileStorage);

        Task<IEnumerable<FileStorage>> GetAllByParentId(int id);
    }
}
