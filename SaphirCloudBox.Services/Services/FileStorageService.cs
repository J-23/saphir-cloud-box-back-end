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
using SaphirCloudBox.Services.Utils;
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

        public async Task AddFile(AddFileDto fileDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();

            var parentFileStorage = await fileStorageRepository.GetById(fileDto.ParentId, userId, clientId);

            if (parentFileStorage == null || !parentFileStorage.IsDirectory)
            {
                throw new NotFoundException();
            }

            var fileName = Path.GetFileNameWithoutExtension(fileDto.Name);
            var fileExtension = Path.GetExtension(fileDto.Name);

            var fileStorages = await fileStorageRepository.GetByParentId(fileDto.ParentId, userId, clientId);

            if (fileStorages.Any(x => !x.IsDirectory && x.Name.Equals(fileName) && x.Files.Any(y => y.IsActive && y.Extension.Equals(fileExtension))))
            {
                throw new FoundSameObjectException();
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roles = await _userManager.GetRolesAsync(user);

            var owners = GetOwners(parentFileStorage, roles, userId, clientId);
            var sizeInfo = fileDto.Size.ToPrettySize();
            var blobName = Guid.NewGuid();

            var newFileStorage = new FileStorage
            {
                Name = fileDto.Name,
                ParentFileStorageId = parentFileStorage.Id,
                IsDirectory = false,
                CreateDate = DateTime.Now,
                CreateById = userId,
                ClientId = owners.ClientId,
                OwnerId = owners.OwnerId,
                Files = new List<Models.File>
                {
                    new Models.File
                    {
                        Extension = fileExtension,
                        Size = sizeInfo.Size,
                        SizeType = sizeInfo.SizeType,
                        IsActive = true,
                        CreateById = userId,
                        CreateDate = DateTime.Now,
                        AzureBlobStorage = new AzureBlobStorage
                        {
                            BlobName = blobName
                        }
                    }
                }
            };

            await _azureBlobClient.UploadFile(_blobSettings.ContainerName, blobName.ToString(), Base64ToByteArray(fileDto.Content));
            
            await fileStorageRepository.Add(newFileStorage);
        }

        public async Task AddFolder(AddFolderDto folderDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();

            var parentFileStorage = await fileStorageRepository.GetById(folderDto.ParentId, userId, clientId);

            if (parentFileStorage == null || parentFileStorage != null && !parentFileStorage.IsDirectory)
            {
                throw new NotFoundException();
            }

            var childFileStorages = await fileStorageRepository.GetByParentId(folderDto.ParentId, userId, clientId);

            if (childFileStorages.Any(x => x.IsDirectory && x.Name.Equals(folderDto.Name)))
            {
                throw new FoundSameObjectException();
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roles = await _userManager.GetRolesAsync(user);

            var owners = GetOwners(parentFileStorage, roles, userId, clientId);

            var newFileStorage = new FileStorage
            {
                IsDirectory = true,
                Name = folderDto.Name,
                CreateDate = DateTime.Now,
                ParentFileStorageId = parentFileStorage.Id,
                CreateById = userId,
                ClientId = owners.ClientId,
                OwnerId = owners.OwnerId
            };

            await fileStorageRepository.Add(newFileStorage);
        }

        public async Task<FileStorageDto> GetByParentId(int parentId, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();

            var parentFileStorage = await fileStorageRepository.GetById(parentId, userId, clientId);

            if (parentFileStorage == null || parentFileStorage != null && !parentFileStorage.IsDirectory)
            {
                throw new NotFoundException();
            }

            var fileStorages = await fileStorageRepository.GetByParentId(parentId, userId, clientId);

            var storages = MapperFactory.CreateMapper<IFileStorageMapper>().MapCollectionToModel(fileStorages);

            return new FileStorageDto
            {
                Id = parentFileStorage.Id,
                ParentStorageId = parentFileStorage.ParentFileStorageId,
                Name = parentFileStorage.Name,
                Client = MapperFactory.CreateMapper<IClientMapper>().MapToModel(parentFileStorage.Client),
                Owner = MapperFactory.CreateMapper<IUserMapper>().MapToModel(parentFileStorage.Owner),
                Storages = storages
            };
        }

        public async Task<DownloadFileDto> GetFileById(int id, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(id, userId, clientId);

            if (fileStorage == null || fileStorage != null && fileStorage.IsDirectory)
            {
                throw new NotFoundException();
            }

            var file = fileStorage.Files.FirstOrDefault(x => x.IsActive);

            if (file == null)
            {
                throw new NotFoundException();
            }

            var buffer = await _azureBlobClient.DownloadFile(_blobSettings.ContainerName, file.AzureBlobStorage.BlobName.ToString());

            return new DownloadFileDto
            {
                Name = fileStorage.Name,
                Buffer = buffer
            };
        }

        public async Task RemoveFile(RemoveFileStorageDto fileDto, int userId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(fileDto.Id, userId, 1);

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

        public async Task RemoveFolder(RemoveFileStorageDto folderDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(folderDto.Id, userId, clientId);

            if (fileStorage == null || fileStorage != null && !fileStorage.IsDirectory)
            {
                throw new NotFoundException();
            }

            await fileStorageRepository.RemoveFolder(fileStorage);
        }

        public async Task UpdateFolder(UpdateFolderDto folderDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(folderDto.Id, userId, clientId);

            if (fileStorage == null || fileStorage != null && !fileStorage.IsDirectory)
            {
                throw new NotFoundException();
            }

            if (fileStorage.ParentFileStorageId.HasValue)
            {
                var childFileStorages = await fileStorageRepository.GetByParentId(fileStorage.ParentFileStorageId.Value, userId, clientId);

                if (childFileStorages.Any(x => x.Name.Equals(childFileStorages) && x.Id != folderDto.Id))
                {
                    throw new FoundSameObjectException();
                }
            }
            
            fileStorage.Name = folderDto.Name;
            fileStorage.UpdateById = userId;
            fileStorage.UpdateDate = DateTime.Now;

            await fileStorageRepository.Update(fileStorage);
        }

        private byte[] Base64ToByteArray(string content)
        {
            byte[] buffer = new byte[((content.Length * 3) + 3) / 4 - (content.Length > 0 && content[content.Length - 1] == '=' ?
                content.Length > 1 && content[content.Length - 2] == '=' ? 2 : 1 : 0)];

            int written;
            if (!Convert.TryFromBase64String(content, buffer, out written))
            {
                //exception
            }

            return buffer;
        }

        private (int? OwnerId, int? ClientId) GetOwners(FileStorage parentFileStorage, IList<string> roles, int userId, int userClientId)
        {
            int? ownerId = null;
            int? clientId = null;

            if (parentFileStorage.Id == 1)
            {
                foreach (var role in roles)
                {
                    if (role.Equals(Role.SUPER_ADMIN_ROLE_NAME))
                    {
                        ownerId = null;
                        clientId = null;
                    }
                    else if (role.Equals(Role.CLIENT_ADMIN_ROLE_NAME))
                    {
                        ownerId = null;
                        clientId = userClientId;
                    }
                    else if (role.Equals(Role.DEPARTMENT_HEAD_ROLE_NAME) || role.Equals(Role.EMPLOYEE_ROLE_NAME))
                    {
                        ownerId = userId;
                        clientId = null;
                    }
                    else
                    {
                        ownerId = userId;
                        clientId = null;
                    }
                }
            }
            else
            {
                ownerId = parentFileStorage.OwnerId;
                clientId = parentFileStorage.ClientId;
            }

            return (ownerId, clientId);
        }
    }
}
