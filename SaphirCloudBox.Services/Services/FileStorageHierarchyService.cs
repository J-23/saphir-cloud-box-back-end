using Anthill.Common.Data.Contracts;
using Anthill.Common.Services;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Mappers;
using SaphirCloudBox.Services.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class FileStorageHierarchyService : AbstractService, IFileStorageHierarchyService
    {
        public FileStorageHierarchyService(IUnityContainer container, 
            ISaphirCloudBoxDataContextManager dataContextManager) : base(container, dataContextManager)
        {
        }

        public async Task<FolderDto> GetByChildId(int childId, int userId, int clientId)
        {
            var fileStorageHierarchyRepository = DataContextManager.CreateRepository<IFileStorageHierarchyRepository>();
            var folderDto = await fileStorageHierarchyRepository.GetByChildId(childId, userId, clientId);

            return MapperFactory.CreateMapper<IFolderMapper>().MapToModel(folderDto);
        }

        public async Task<IEnumerable<FolderDto>> GetByParentId(int parentId, int userId, int clientId)
        {
            var fileStorageHierarchyRepository = DataContextManager.CreateRepository<IFileStorageHierarchyRepository>();

            var fileStorages = await fileStorageHierarchyRepository.GetByParentId(parentId, userId, clientId);

            var folderDtos = MapperFactory.CreateMapper<IFolderMapper>().MapCollectionToModel(fileStorages);

            await SetChildren(folderDtos, userId, clientId);

            return folderDtos;
        }

        private async Task SetChildren(IEnumerable<FolderDto> folders, int userId, int clientId)
        {
            var fileStorageHierarchyRepository = DataContextManager.CreateRepository<IFileStorageHierarchyRepository>();

            foreach (var folder in folders)
            {
                var children = await fileStorageHierarchyRepository.GetByParentId(folder.Id, userId, clientId);
                folder.Children = MapperFactory.CreateMapper<IFolderMapper>().MapCollectionToModel(children);
                folder.NewFileCount = await fileStorageHierarchyRepository.GetNewFileCountByParentId(folder.Id, userId, clientId);

                await SetChildren(folder.Children, userId, clientId);
            }
        }
    }
}
