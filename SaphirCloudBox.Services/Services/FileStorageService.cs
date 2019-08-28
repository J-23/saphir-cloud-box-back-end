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
using SaphirCloudBox.Services.Contracts.Dtos.Permission;
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
using System.Transactions;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class FileStorageService : AbstractService, IFileStorageService
    {
        private readonly IUserService _userService;

        private readonly BlobSettings _blobSettings;
        private readonly AzureBlobClient _azureBlobClient;

        private readonly IPermissionHelper _permissionHelper;

        public FileStorageService(IUnityContainer container, 
            ISaphirCloudBoxDataContextManager dataContextManager, 
            BlobSettings blobSettings,
            AzureBlobClient azureBlobClient,
            IPermissionHelper permissionHelper,
            IUserService userService) 
            : base(container, dataContextManager)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _blobSettings = blobSettings ?? throw new ArgumentNullException(nameof(blobSettings));
            _azureBlobClient = azureBlobClient ?? throw new ArgumentNullException(nameof(azureBlobClient));
            _permissionHelper = permissionHelper ?? throw new ArgumentNullException(nameof(permissionHelper));
        }

        public async Task AddFile(AddFileDto fileDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();

            var parentFileStorage = await fileStorageRepository.GetById(fileDto.ParentId, userId, clientId);

            if (parentFileStorage == null || !parentFileStorage.IsDirectory)
            {
                throw new NotFoundException("File storage", fileDto.ParentId);
            }

            var fileName = Path.GetFileNameWithoutExtension(fileDto.Name);
            var fileExtension = Path.GetExtension(fileDto.Name);

            var childFileStorages = await fileStorageRepository.GetByParentId(parentFileStorage.Id, userId, clientId);

            if (childFileStorages.Any(x => !x.IsDirectory && x.Name.Equals(fileName) 
                && x.Files.Any(y => y.IsActive && y.Extension.Equals(fileExtension))))
            {
                throw new FoundSameObjectException("File storage", fileName);
            }

            var owners = await _permissionHelper.GetOwners(parentFileStorage, userId, clientId);
            var sizeInfo = fileDto.Size.ToPrettySize();
            var blobName = Guid.NewGuid();

            var newFileStorage = new FileStorage
            {
                Name = fileName,
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

            await _azureBlobClient.UploadFile(_blobSettings.ContainerName, blobName.ToString(), fileDto.Content.ToByteArray());
            
            await fileStorageRepository.Add(newFileStorage);
        }

        public async Task AddFolder(AddFolderDto folderDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();

            var parentFileStorage = await fileStorageRepository.GetById(folderDto.ParentId, userId, clientId);

            if (parentFileStorage == null || parentFileStorage != null && !parentFileStorage.IsDirectory)
            {
                throw new NotFoundException("File storage", folderDto.ParentId);
            }

            var childFileStorages = await fileStorageRepository.GetByParentId(parentFileStorage.Id, userId, clientId);

            if (childFileStorages.Any(x => x.IsDirectory && x.Name.Equals(folderDto.Name)))
            {
                throw new FoundSameObjectException("File storage", folderDto.Name);
            }

            var owners = await _permissionHelper.GetOwners(parentFileStorage, userId, clientId);

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
                throw new NotFoundException("File storage", parentId);
            }

            var childFileStorages = await fileStorageRepository.GetByParentId(parentFileStorage.Id, userId, clientId);

            var storages = MapperFactory.CreateMapper<IFileStorageMapper>().MapCollectionToModel(childFileStorages);

            return new FileStorageDto
            {
                Id = parentFileStorage.Id,
                Name = parentFileStorage.Name,
                ParentStorageId = parentFileStorage.ParentFileStorageId,
                Client = MapperFactory.CreateMapper<IClientMapper>().MapToModel(parentFileStorage.Client),
                Owner = MapperFactory.CreateMapper<IUserMapper>().MapToModel(parentFileStorage.Owner),
                Permissions = MapperFactory.CreateMapper<IPermissionMapper>()
                    .MapCollectionToModel(parentFileStorage.Permissions.Where(x => !x.EndDate.HasValue)),
                Storages = storages
            };
        }

        public async Task<DownloadFileDto> GetFileById(int id, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(id, userId, clientId);

            if (fileStorage == null || fileStorage != null && fileStorage.IsDirectory)
            {
                throw new NotFoundException("File storage", id);
            }

            var file = fileStorage.Files.FirstOrDefault(x => x.IsActive);

            if (file == null)
            {
                throw new NotFoundDependencyObjectException("File");
            }

            var buffer = await _azureBlobClient.DownloadFile(_blobSettings.ContainerName, file.AzureBlobStorage.BlobName.ToString());

            return new DownloadFileDto
            {
                Name = fileStorage.Name,
                Extension = file.Extension,
                Buffer = buffer
            };
        }

        public async Task RemoveFile(RemoveFileStorageDto fileDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(fileDto.Id, userId, 1);

            if (fileStorage == null || fileStorage != null && fileStorage.IsDirectory)
            {
                throw new NotFoundException("File storage", fileDto.Id);
            }

            var isAvailableToChange = await fileStorageRepository.IsAvailableToChange(fileStorage.Id, userId, clientId);

            if (!isAvailableToChange)
            {
                throw new UnavailableOperationException("remove file", "File storage", fileStorage.Id, userId);
            }

            foreach (var file in fileStorage.Files)
            {
                await _azureBlobClient.DeleteFile(_blobSettings.ContainerName, file.AzureBlobStorage.BlobName.ToString());
            }

            await fileStorageRepository.Remove(fileStorage);
        }

        public async Task RemoveFolder(RemoveFileStorageDto folderDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(folderDto.Id, userId, clientId);

            if (fileStorage == null || fileStorage != null && !fileStorage.IsDirectory)
            {
                throw new NotFoundException("File storage", folderDto.Id);
            }

            var isAvailableToChange = await fileStorageRepository.IsAvailableToChange(fileStorage.Id, userId, clientId);

            if (!isAvailableToChange)
            {
                throw new UnavailableOperationException("remove folder", "File storage", fileStorage.Id, userId);
            }

            var files = await fileStorageRepository.GetFilesByParentId(fileStorage.Id, userId, clientId);

            var blobs = files.SelectMany(s => s.Files).Select(s => s.AzureBlobStorage).ToList();

            foreach (var blob in blobs)
            {
                await _azureBlobClient.DeleteFile(_blobSettings.ContainerName, blob.BlobName.ToString());
            }

            await fileStorageRepository.RemoveFolder(fileStorage);
        }

        public async Task UpdateFolder(UpdateFolderDto folderDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(folderDto.Id, userId, clientId);

            if (fileStorage == null || fileStorage != null && !fileStorage.IsDirectory)
            {
                throw new NotFoundException("File storage", folderDto.Id);
            }

            var isAvailableToChange = await fileStorageRepository.IsAvailableToChange(fileStorage.Id, userId, clientId);

            if (!isAvailableToChange)
            {
                throw new UnavailableOperationException("update folder", "File storage", fileStorage.Id, userId);
            }

            if (fileStorage.ParentFileStorageId.HasValue)
            {
                var childFileStorages = await fileStorageRepository.GetByParentId(fileStorage.ParentFileStorageId.Value, userId, clientId);

                if (childFileStorages.Any(x => x.Name.Equals(folderDto.Name) && x.Id != folderDto.Id))
                {
                    throw new FoundSameObjectException("File storage", folderDto.Name);
                }
            }
            
            fileStorage.Name = folderDto.Name;
            fileStorage.UpdateById = userId;
            fileStorage.UpdateDate = DateTime.Now;

            await fileStorageRepository.Update(fileStorage);
        }

        public async Task UpdateFile(UpdateFileDto fileDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(fileDto.Id, userId, clientId);

            if (fileStorage == null || fileStorage != null && fileStorage.IsDirectory)
            {
                throw new NotFoundException("File storage", fileDto.Id);
            }

            var isAvailableToChange = await fileStorageRepository.IsAvailableToChange(fileStorage.Id, userId, clientId);

            if (!isAvailableToChange)
            {
                throw new UnavailableOperationException("update file", "File storage", fileStorage.Id, userId);
            }

            var fileName = Path.GetFileNameWithoutExtension(fileDto.Name);
            var fileExtension = Path.GetExtension(fileDto.Name);

            if (fileStorage.ParentFileStorageId.HasValue)
            {
                var fileStorages = await fileStorageRepository.GetByParentId(fileStorage.ParentFileStorageId.Value, userId, clientId);

                if (fileStorages.Any(x => !x.IsDirectory && x.Name.Equals(fileName) && x.Files.Any(y => y.IsActive && y.Extension.Equals(fileExtension))))
                {
                    throw new FoundSameObjectException("File storage", fileName);
                }
            }

            fileStorage.Name = fileName;
            fileStorage.UpdateDate = DateTime.Now;
            fileStorage.UpdateById = userId;

            if (!String.IsNullOrEmpty(fileDto.Content))
            {
                fileStorage.Files.ToList().ForEach(file =>
                {
                    file.IsActive = false;
                });

                var sizeInfo = fileDto.Size.Value.ToPrettySize();
                var blobName = Guid.NewGuid();

                var newFile = new Models.File
                {
                    Extension = fileExtension,
                    Size = sizeInfo.Size,
                    SizeType = sizeInfo.SizeType,
                    CreateById = userId,
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    AzureBlobStorage = new AzureBlobStorage
                    {
                        BlobName = blobName
                    }
                };

                fileStorage.Files.Add(newFile);

                await _azureBlobClient.UploadFile(_blobSettings.ContainerName, blobName.ToString(), fileDto.Content.ToByteArray());
            }
            else
            {
                var activeFile = fileStorage.Files.FirstOrDefault();

                if (activeFile == null)
                {
                    throw new NotFoundDependencyObjectException("File");
                }

                activeFile.Extension = fileExtension;
                activeFile.UpdateById = userId;
                activeFile.UpdateDate = DateTime.Now;
            }

            await fileStorageRepository.Update(fileStorage);
        }

        public async Task<CheckPermissionResultDto> CheckPermission(CheckPermissionDto permissionDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(permissionDto.FileStorageId, userId, clientId);

            var isAvailableToChange = await fileStorageRepository.IsAvailableToChange(fileStorage.Id, userId, clientId);

            if (!isAvailableToChange)
            {
                throw new UnavailableOperationException("update permission", "Permission", fileStorage.Id, userId);
            }

            var sender = await _userService.GetById(userId);

            var users = await GetUsers(permissionDto.ClientIds, permissionDto.UserIds, sender.Id);

            var parentFileStorages = await fileStorageRepository.GetParents(fileStorage.ParentFileStorageId, sender.Id, clientId);

            var recipients = new List<CheckPermissionResultDto.RecipientDto>();

            fileStorage.Permissions.Where(x => users.Select(s => s.Id).Contains(x.RecipientId) && !x.EndDate.HasValue)
                .ToList()
                .ForEach(perm =>
                {
                    perm.Type = permissionDto.Type;
                    recipients.Add(new CheckPermissionResultDto.RecipientDto
                    {
                        Id = perm.Recipient.Id,
                        UserName = perm.Recipient.UserName,
                        Email = perm.Recipient.Email,
                        Type = CheckPermissionResultDto.UpdateType.Update
                    });
                });

            users.Where(x => !fileStorage.Permissions.Any(y => y.RecipientId == x.Id && !y.EndDate.HasValue))
                .ToList()
                .ForEach(user =>
                {
                    fileStorage.Permissions.Add(new FileStoragePermission
                    {
                        RecipientId = user.Id,
                        SenderId = sender.Id,
                        Type = permissionDto.Type,
                        StartDate = DateTime.Now
                    });

                    recipients.Add(new CheckPermissionResultDto.RecipientDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Type = CheckPermissionResultDto.UpdateType.Add
                    });
                });

            fileStorage.Permissions.Where(x => !users.Select(s => s.Id).Contains(x.RecipientId) && !x.EndDate.HasValue)
                .ToList()
                .ForEach(perm =>
                {
                    perm.EndDate = DateTime.Now;
                    recipients.Add(new CheckPermissionResultDto.RecipientDto
                    {
                        Id = perm.Recipient.Id,
                        UserName = perm.Recipient.UserName,
                        Email = perm.Recipient.Email,
                        Type = CheckPermissionResultDto.UpdateType.Remove
                    });
                });

            if (!fileStorage.ClientId.HasValue && !fileStorage.OwnerId.HasValue || fileStorage.ClientId.HasValue && !fileStorage.OwnerId.HasValue)
            {
                parentFileStorages.ToList()
                    .ForEach(storage =>
                    {
                        storage.Permissions.Where(x => users.Select(s => s.Id).Contains(x.RecipientId) && !x.EndDate.HasValue)
                            .ToList()
                            .ForEach(perm =>
                            {
                                perm.Type = permissionDto.Type;
                            });

                        users.Where(x => !fileStorage.Permissions.Any(y => y.RecipientId == x.Id && !y.EndDate.HasValue))
                            .ToList()
                            .ForEach(user =>
                            {
                                storage.Permissions.Add(new FileStoragePermission
                                {
                                    RecipientId = user.Id,
                                    SenderId = sender.Id,
                                    Type = permissionDto.Type,
                                    StartDate = DateTime.Now
                                });
                            });

                        storage.Permissions.Where(x => !users.Select(s => s.Id).Contains(x.RecipientId) && !x.EndDate.HasValue)
                            .ToList()
                            .ForEach(perm =>
                            {
                                perm.EndDate = DateTime.Now;
                            });
                    });
            }

            await SaveChanges(fileStorageRepository, fileStorage, parentFileStorages);

            return new CheckPermissionResultDto
            {
                Storage = MapperFactory.CreateMapper<IFileStorageMapper>().MapToModel(fileStorage),
                Sender = sender,
                Recipients = recipients
            };
        }

        private async Task SaveChanges(IFileStorageRepository fileStorageRepository, FileStorage fileStorage, IEnumerable<FileStorage> parentFileStorages)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await fileStorageRepository.Update(fileStorage);
                    await fileStorageRepository.Update(parentFileStorages);
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message, ex);
                }

                scope.Complete();
            }
        }

        public async Task<CheckPermissionResultDto> UpdatePermission(UpdatePermissionDto permissionDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(permissionDto.FileStorageId, userId, clientId);

            var isAvailableToChange = await fileStorageRepository.IsAvailableToChange(fileStorage.Id, userId, clientId);

            if (!isAvailableToChange)
            {
                throw new UnavailableOperationException("update permission", "Permission", fileStorage.Id, userId);
            }

            var sender = await _userService.GetById(userId);
            var recipient = await _userService.GetById(permissionDto.RecipientId);

            var fileStoragePermission = fileStorage.Permissions.FirstOrDefault(x => x.RecipientId == recipient.Id && !x.EndDate.HasValue);

            if (fileStoragePermission == null)
            {
                throw new NotFoundException("File storage permission", fileStorage.Id, recipient.Email);
            }

            fileStoragePermission.Type = permissionDto.Type;
            var parentFileStorages = await fileStorageRepository.GetParents(fileStorage.ParentFileStorageId, userId, clientId);

            if (!fileStorage.ClientId.HasValue && !fileStorage.OwnerId.HasValue || fileStorage.ClientId.HasValue && !fileStorage.OwnerId.HasValue)
            {
                parentFileStorages.ToList()
                    .ForEach(storage =>
                    {
                        var permission = storage.Permissions.FirstOrDefault(x => x.RecipientId == recipient.Id && !x.EndDate.HasValue);

                        if (permission != null)
                        {
                            permission.Type = permissionDto.Type;
                        }
                        else
                        {
                            storage.Permissions.Add(new FileStoragePermission
                            {
                                RecipientId = recipient.Id,
                                SenderId = userId,
                                Type = permissionDto.Type,
                                StartDate = DateTime.Now
                            });
                        }
                    });
            }

            await SaveChanges(fileStorageRepository, fileStorage, parentFileStorages);

            return new CheckPermissionResultDto
            {
                Storage = MapperFactory.CreateMapper<IFileStorageMapper>().MapToModel(fileStorage),
                Sender = sender,
                Recipients = new List<CheckPermissionResultDto.RecipientDto>
                {
                    new CheckPermissionResultDto.RecipientDto
                    {
                        Id = recipient.Id,
                        UserName = recipient.UserName,
                        Email = recipient.Email,
                        Type = CheckPermissionResultDto.UpdateType.Update
                    }
                }
            };
        }

        public async Task<CheckPermissionResultDto> RemovePermission(RemovePermissionDto permissionDto, int userId, int clientId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();
            var fileStorage = await fileStorageRepository.GetById(permissionDto.FileStorageId, userId, clientId);

            var isAvailableToChange = await fileStorageRepository.IsAvailableToChange(fileStorage.Id, userId, clientId);

            if (!isAvailableToChange)
            {
                throw new UnavailableOperationException("remove permission", "Permission", fileStorage.Id, userId);
            }

            var sender = await _userService.GetById(userId);
            var recipient = await _userService.GetById(permissionDto.RecipientId);

            var fileStoragePermission = fileStorage.Permissions.FirstOrDefault(x => x.RecipientId == recipient.Id && !x.EndDate.HasValue);

            if (fileStoragePermission == null)
            {
                throw new NotFoundException("File storage permission", fileStorage.Id, recipient.Email);
            }

            fileStoragePermission.EndDate = DateTime.Now;

            var parentFileStorages = await fileStorageRepository.GetParents(fileStorage.ParentFileStorageId, userId, clientId);

            if (!fileStorage.ClientId.HasValue && !fileStorage.OwnerId.HasValue || fileStorage.ClientId.HasValue && !fileStorage.OwnerId.HasValue)
            {
                parentFileStorages.ToList()
                    .ForEach(storage =>
                    {
                        var permission = storage.Permissions.FirstOrDefault(x => x.RecipientId == recipient.Id && !x.EndDate.HasValue);

                        if (permission != null)
                        {
                            permission.EndDate = DateTime.Now;
                        }
                    });
            }

            await SaveChanges(fileStorageRepository, fileStorage, parentFileStorages);

            return new CheckPermissionResultDto
            {
                Storage = MapperFactory.CreateMapper<IFileStorageMapper>().MapToModel(fileStorage),
                Sender = sender,
                Recipients = new List<CheckPermissionResultDto.RecipientDto>
                {
                    new CheckPermissionResultDto.RecipientDto
                    {
                        Id = recipient.Id,
                        UserName = recipient.UserName,
                        Email = recipient.Email,
                        Type = CheckPermissionResultDto.UpdateType.Remove
                    }
                }
            };
        }

        private async Task<IEnumerable<UserDto>> GetUsers(IEnumerable<int> clientIds, IEnumerable<int> userIds, int userId)
        {
            var users = (await _userService.GetByClientIds(clientIds)).ToList();

            userIds = userIds.Where(x => !users.Select(s => s.Id).Contains(x));

            users.AddRange(await _userService.GetByIds(userIds));

            return users.Where(x => x.Id != userId).ToList();
        }

        public async Task<IEnumerable<FileStorageDto.StorageDto>> GetSharedFiles(int userId)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();

            var fileStorages = await fileStorageRepository.GetSharedFiles(userId);

            return MapperFactory.CreateMapper<IFileStorageMapper>().MapCollectionToModel(fileStorages);
        }
    }
}
