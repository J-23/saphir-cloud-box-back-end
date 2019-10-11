using Anthill.Common.Data.Contracts;
using Anthill.Common.Services;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos.UserGroup;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Mappers;
using SaphirCloudBox.Services.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class UserGroupService : AbstractService, IUserGroupService
    {
        private readonly IUserService _userService;

        public UserGroupService(IUnityContainer container, 
            ISaphirCloudBoxDataContextManager dataContextManager,
            IUserService userService) 
            : base(container, dataContextManager)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<int> Add(AddUserGroupDto groupDto, int userId)
        {
            var userGroupRepository = DataContextManager.CreateRepository<IUserGroupRepository>();

            var group = await userGroupRepository.GetByName(groupDto.Name, userId);

            if (group != null)
            {
                throw new FoundSameObjectException("Group", groupDto.Name);
            }

            var users = await _userService.GetByIds(groupDto.UserIds);
            var newGroup = new Group
            {
                OwnerId = userId,
                Name = groupDto.Name,
                UsersInGroup = users.Select(us => new UserInGroup
                {
                    UserId = us.Id
                })
                .ToList(),
                IsActive = true
            };

            await userGroupRepository.Add(newGroup);

            return newGroup.Id;
        }

        public async Task<UserGroupDto> GetById(int groupId, int userId)
        {
            var userGroupRepository = DataContextManager.CreateRepository<IUserGroupRepository>();
            var group = await userGroupRepository.GetById(groupId, userId);

            if (group == null)
            {
                throw new NotFoundException("Group", groupId);
            }

            return MapperFactory.CreateMapper<IUserGroupMapper>().MapToModel(group);
        }

        public async Task<IEnumerable<UserGroupDto>> GetGroups(int userId)
        {
            var userGroupRepository = DataContextManager.CreateRepository<IUserGroupRepository>();

            var groups = await userGroupRepository.GetGroups(userId);
            return MapperFactory.CreateMapper<IUserGroupMapper>().MapCollectionToModel(groups);
        }

        public async Task Remove(RemoveUserGroupDto groupDto, int userId)
        {
            var userGroupRepository = DataContextManager.CreateRepository<IUserGroupRepository>();

            var group = await userGroupRepository.GetById(groupDto.Id, userId);

            if (group == null)
            {
                throw new NotFoundException("Group", groupDto.Id);
            }

            group.IsActive = false;
            await userGroupRepository.Update(group);
        }

        public async Task Update(UpdateUserGroupDto groupDto, int userId)
        {
            var userGroupRepository = DataContextManager.CreateRepository<IUserGroupRepository>();

            var group = await userGroupRepository.GetById(groupDto.Id, userId);

            if (group == null)
            {
                throw new NotFoundException("Group", groupDto.Id);
            }

            var otherGroup = await userGroupRepository.GetByName(groupDto.Name, userId);

            if (otherGroup != null && otherGroup.Id != group.Id)
            {
                throw new FoundSameObjectException("Group", groupDto.Name);
            }

            var users = await _userService.GetByIds(groupDto.UserIds);

            users.Where(x => !group.UsersInGroup.Any(y => y.UserId == x.Id))
                .ToList()
                .ForEach(us =>
                {
                    group.UsersInGroup.Add(new UserInGroup
                    {
                        UserId = us.Id
                    });
                });

            group.UsersInGroup = group.UsersInGroup.Where(x => users.Any(y => y.Id == x.UserId)).ToList();

            group.Name = groupDto.Name;

            await userGroupRepository.Update(group);
        }
    }
}
