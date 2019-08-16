using SaphirCloudBox.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Contracts.Services
{
    public interface IFileStorageService
    {
        Task<FileStorageDto> GetByParentId(int parentId, int userId, int clientId);

        Task AddFolder(AddFolderDto folderDto, int userId, int clientId);

        Task AddFile(AddFileDto fileDto, int userId, int clientId);

        Task<DownloadFileDto> GetFileById(int id, int userId, int clientId);

        Task RemoveFile(RemoveFileStorageDto fileDto, int userId, int clientId);

        Task RemoveFolder(RemoveFileStorageDto folderDto, int userId, int clientId);

        Task UpdateFolder(UpdateFolderDto folderDto, int userId, int clientId);

        Task UpdateFile(UpdateFileDto fileDto, int userId, int clientId);
    }
}
