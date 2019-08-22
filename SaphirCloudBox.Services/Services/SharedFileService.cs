using Anthill.Common.Data.Contracts;
using Anthill.Common.Services;
using Microsoft.AspNetCore.Identity;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Dtos.Permission;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Mappers;
using SaphirCloudBox.Services.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class SharedFileService : AbstractService, ISharedFileService
    {
        private readonly IUserService _userService;

        public SharedFileService(IUnityContainer container, 
            ISaphirCloudBoxDataContextManager dataContextManager,
            IUserService userService) 
            : base(container, dataContextManager)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task AddPermission(AddPermissionDto permissionDto, int userId)
        {
            var fileStorageAccessRepository = DataContextManager.CreateRepository<IFileStoragePermissionRepository>();

            var isAvailable = await fileStorageAccessRepository.IsAvailable(userId, PermissionType.ReadAndWrite);

            if (!isAvailable)
            {
                throw new AddException();
            }

            var recipient = await _userService.GetByEmail(permissionDto.RecipientEmail);
            var permission = new FileStoragePermission
            {
                FileStorageId = permissionDto.FileStorageId,
                SenderId = userId,
                RecipientId = recipient.Id,
                Type = permissionDto.Type,
                StartDate = DateTime.Now
            };

            await fileStorageAccessRepository.Add(permission);
        }

        public async Task<IEnumerable<SharedFileDto>> GetByUserId(int userId)
        {
            var fileStorageAccessRepository = DataContextManager.CreateRepository<IFileStoragePermissionRepository>();

            var fileStoragePermissions = await fileStorageAccessRepository.GetByUserId(userId);

            return MapperFactory.CreateMapper<ISharedFileMapper>().MapCollectionToModel(fileStoragePermissions);
        }
    }
}
