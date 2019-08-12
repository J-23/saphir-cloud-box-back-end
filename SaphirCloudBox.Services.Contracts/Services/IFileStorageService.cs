using SaphirCloudBox.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Contracts.Services
{
    public interface IFileStorageService
    {
        Task<IEnumerable<FolderDto>> GetByParentId(int parentId, int userId);

        Task AddFolder(AddFolderDto folderDto, int userId);

        Task AddFile(AddFileDto fileDto, int userId);

        Task<DownloadFileDto> GetFileById(int id, int userId);

        Task RemoveFile(RemoveFileDto fileDto, int userId);
    }
}
