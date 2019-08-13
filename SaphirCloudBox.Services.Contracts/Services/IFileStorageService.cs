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

        Task AddFile(AddFileDto fileDto, int userId);

        Task<DownloadFileDto> GetFileById(int id, int userId);

        Task RemoveFile(RemoveFileDto fileDto, int userId);
    }
}
