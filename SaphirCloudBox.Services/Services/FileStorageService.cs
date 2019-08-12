using Anthill.Common.AzureBlob;
using Anthill.Common.Data.Contracts;
using Anthill.Common.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Mappers;
using SaphirCloudBox.Services.Contracts.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class FileStorageService : AbstractService, IFileStorageService
    {
        private readonly UserManager<User> _userManager;
        private readonly BlobSettings _blobSettings;
        private readonly AzureBlobClient _azureBlobClient;

        public FileStorageService(IUnityContainer container, 
            ISaphirCloudBoxDataContextManager dataContextManager, 
            UserManager<User> userManager,
            BlobSettings blobSettings,
            AzureBlobClient azureBlobClient) 
            : base(container, dataContextManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _blobSettings = blobSettings ?? throw new ArgumentNullException(nameof(blobSettings));
            _azureBlobClient = azureBlobClient ?? throw new ArgumentNullException(nameof(azureBlobClient));
        }

        public async Task AddFile(AddFileDto fileDto, int userId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();

            var parentFileStorage = await fileStorageRepository.GetById(fileDto.ParentId, userId);

            if (parentFileStorage == null)
            {
                throw new NotFoundException();
            }

            if (!parentFileStorage.IsDirectory)
            {
                throw new NotFoundException();
            }

            var fileStorages = await fileStorageRepository.GetByParentId(fileDto.ParentId, userId);

            if (fileStorages.Any(x => x.IsDirectory && x.Name.Equals(fileDto.Name)))
            {
                throw new FoundSameObjectException();
            }

            var newFileStorage = new FileStorage
            {
                IsDirectory = false,
                Name = fileDto.Name,
                BlobName = Guid.NewGuid(),
                CreateDate = DateTime.Now,
                ParentFileStorageId = parentFileStorage.Id,
                FileStorageAccesses = await GetAccessUsers(userId, parentFileStorage.AccessType)
            };

            await _azureBlobClient.UploadFile(_blobSettings.ContainerName, newFileStorage.BlobName.ToString(), Base64ToByteArray(fileDto.Content));
            
            await fileStorageRepository.Add(newFileStorage);
        }

        public async Task AddFolder(AddFolderDto folderDto, int userId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();

            var parentFileStorage = await fileStorageRepository.GetById(folderDto.ParentId, userId);

            if (parentFileStorage == null)
            {
                throw new NotFoundException();
            }

            if (!parentFileStorage.IsDirectory)
            {
                throw new NotFoundException();
            }

            var fileStorages = await fileStorageRepository.GetByParentId(folderDto.ParentId, userId);

            if (fileStorages.Any(x => x.IsDirectory && x.Name.Equals(folderDto.Name)))
            {
                throw new FoundSameObjectException();
            }

            var newFileStorage = new FileStorage
            {
                IsDirectory = true,
                Name = folderDto.Name,
                CreateDate = DateTime.Now,
                ParentFileStorageId = parentFileStorage.Id,
                AccessType = parentFileStorage.AccessType,
                FileStorageAccesses = await GetAccessUsers(userId, parentFileStorage.AccessType)
            };

            await fileStorageRepository.Add(newFileStorage);
        }

        public async Task<IEnumerable<FolderDto>> GetByParentId(int parentId, int userId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();

            var fileStorages = await fileStorageRepository.GetByParentId(parentId, userId);

            return ConvertModelsToDtos(fileStorages);
        }

        public async Task<DownloadFileDto> GetFileById(int id, int userId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(id, userId);

            if (fileStorage == null)
            {
                throw new NotFoundException();
            }

            if (fileStorage.IsDirectory)
            {
                throw new NotFoundException();
            }

            var buffer = await _azureBlobClient.DownloadFile(_blobSettings.ContainerName, fileStorage.BlobName.ToString());

            return new DownloadFileDto
            {
                Name = fileStorage.Name,
                Buffer = buffer
            };
        }

        public async Task RemoveFile(RemoveFileDto fileDto, int userId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(fileDto.Id, userId);

            if (fileStorage == null)
            {
                throw new NotFoundException();
            }

            if (fileStorage.IsDirectory)
            {
                throw new NotFoundException();
            }

            await _azureBlobClient.DeleteFile(_blobSettings.ContainerName, fileStorage.Id.ToString());

            await fileStorageRepository.Remove(fileStorage);
        }

        private IEnumerable<FolderDto> ConvertModelsToDtos(IEnumerable<FileStorage> fileStorages)
        {
            var folders = MapperFactory.CreateMapper<IFolderMapper>().MapCollectionToModel(fileStorages.Where(x => x.IsDirectory));

            var fileMapper = MapperFactory.CreateMapper<IFileMapper>();

            var fileGroups = fileStorages.Where(x => !x.IsDirectory).GroupBy(grp => grp.ParentFileStorageId).ToList();

            foreach (var fileGroup in fileGroups)
            {
                var files = fileMapper.MapCollectionToModel(fileGroup.ToList());

                var folder = folders.FirstOrDefault(x => x.Id == fileGroup.Key);
                folder.Files = files;
            }

            return folders;
        }

        private async Task<IEnumerable<FileStorageAccess>> GetAccessUsers(int userId, AccessType accessType)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                //exception
            }

            var userIds = new List<int>();

            if (accessType == AccessType.Common)
            {
                userIds = await _userManager.Users.Select(s => s.Id).ToListAsync();
            }
            else if (accessType == AccessType.Client)
            {
                var clientRepository = DataContextManager.CreateRepository<IClientRepository>();

                var client = await clientRepository.GetById(user.ClientId);

                if (client == null)
                {
                    // exception
                }

                var userRepository = DataContextManager.CreateRepository<IUserRepository>();
                var users = await userRepository.GetUsersByClientId(client.Id);
                userIds = users.Select(s => s.Id).ToList();
            }
            else if (accessType == AccessType.User)
            {
                userIds.Add(user.Id);
            }

            return userIds
                .Select(usId => new FileStorageAccess
                {
                    UserId = usId
                })
                .ToList();
        }

        private byte[] Base64ToByteArray(string content)
        {
            byte[] buffer = new byte[((content.Length * 3) + 3) / 4 - (content.Length > 0 && content[content.Length - 1] == '=' ?
                content.Length > 1 && content[content.Length - 2] == '=' ? 2 : 1 : 0)];

            int written;
            if(!Convert.TryFromBase64String(content, buffer, out written))
            {
                //exception
            }

            return buffer;
        }
    }
}
