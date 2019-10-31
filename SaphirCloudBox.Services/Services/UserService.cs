using Anthill.Common.Data.Contracts;
using Anthill.Common.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaphirCloudBox.Data;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Mappers;
using SaphirCloudBox.Services.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class UserService : AbstractService, IUserService
    {
        private readonly SaphirUserManager _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;

        public UserService(IUnityContainer container,
            ISaphirCloudBoxDataContextManager dataContextManager,
            SaphirUserManager userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager) : base(container, dataContextManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task Add(AddUserDto userDto, string password)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(userDto.Email) && x.IsActive);

            if (user != null)
            {
                throw new FoundSameObjectException("User", userDto.Email);
            }

            var newUser = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                CreateDate = DateTime.UtcNow,
                ClientId = (await GetClientById(userDto.ClientId)).Id,
                DepartmentId = (await GetDepartmentById(userDto.DepartmentId))?.Id,
                IsActive = true
            };

            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == userDto.RoleId && x.IsActive);

            if (role == null)
            {
                throw new NotFoundDependencyObjectException("Role", userDto.RoleId);
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _userManager.CreateAsync(newUser, password);

                if (!result.Succeeded)
                {
                    scope.Dispose();
                    throw new UserManagerException("add", newUser.Email);
                }

                try
                {
                    await AddUserToRole(newUser, role);
                }
                catch (UserManagerException umex)
                {
                    scope.Dispose();
                    throw new UserManagerException(umex.Message);
                }

                scope.Complete();
            }

        }

        public async Task<string> ForgotPassword(string email)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(email) && x.IsActive);

            if (user == null)
            {
                throw new AppUnauthorizedAccessException(UnautorizedType.NOT_FOUND, ObjectType.USER);
            }

            user.ResetPasswordCode = GenerateForgotPassowrdCodeToken();

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new UserManagerException("forgot password", email);
            }

            return user.ResetPasswordCode;
        }

        public async Task<IEnumerable<UserDto>> GetAll(int userId, int clientId)
        {
            var currentUser = await GetById(userId);

            var users = new List<User>();

            var allClientUsers = await _userManager.Users.Where(x => x.ClientId == currentUser.Client.Id && x.IsActive).ToListAsync();
            var clientAdminRole = await _roleManager.Roles.FirstOrDefaultAsync(x => x.RoleType == RoleType.ClientAdmin);

            switch (currentUser.Role.RoleType)
            {
                case RoleType.SuperAdmin:
                    users = await _userManager.Users.Where(x => x.IsActive).ToListAsync();
                    break;
                case RoleType.ClientAdmin:
                    users = await _userManager.Users.Where(x => x.ClientId == currentUser.Client.Id && x.IsActive).ToListAsync();
                    break;
                case RoleType.DepartmentHead:

                    if (currentUser.Department != null)
                    {
                        users.AddRange(await _userManager.Users.Where(x => x.ClientId == currentUser.Client.Id && x.DepartmentId == currentUser.Department.Id && x.IsActive).ToListAsync());
                    }

                    foreach (var user in allClientUsers)
                    {
                        var isInRole = await _userManager.IsInRoleAsync(user, clientAdminRole.Name);

                        if (isInRole)
                        {
                            users.Add(user);
                        }
                    }

                    break;
                case RoleType.Employee:

                    if (currentUser.Department != null)
                    {
                        users.AddRange(await _userManager.Users.Where(x => x.ClientId == currentUser.Client.Id && x.DepartmentId == currentUser.Department.Id && x.IsActive).ToListAsync());
                    }

                    foreach (var user in allClientUsers)
                    {
                        var isInRole = await _userManager.IsInRoleAsync(user, clientAdminRole.Name);

                        if (isInRole)
                        {
                            users.Add(user);
                        }
                    }

                    break;
                default:
                    break;
            }

            users = users.OrderByDescending(ord => ord.CreateDate)
                .ThenByDescending(ord => ord.UpdateDate)
                .ToList();

            var roles = await _roleManager.Roles.Where(x => x.IsActive).ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userDto = MapperFactory.CreateMapper<IUserMapper>().MapToModel(user);

                var userRoles = await _userManager.GetRolesAsync(user);

                userDto.Role = roles.Where(x => userRoles.Contains(x.Name))
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name
                    })
                    .FirstOrDefault();

                userDtos.Add(userDto);
            }

            return userDtos.OrderBy(ord => ord.UserName).ToList();
        }

        public async Task<UserDto> GetByEmail(string email)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(email) && x.IsActive);

            if (user == null)
            {
                throw new AppUnauthorizedAccessException(UnautorizedType.NOT_FOUND, ObjectType.USER);
            }

            return MapperFactory.CreateMapper<IUserMapper>().MapToModel(user);
        }

        public async Task<UserDto> GetById(int userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId && x.IsActive);

            if (user == null)
            {
                throw new AppUnauthorizedAccessException(UnautorizedType.NOT_FOUND, ObjectType.USER);
            }

            var userDto = MapperFactory.CreateMapper<IUserMapper>().MapToModel(user);

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Count > 0)
            {
                var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Name.Equals(userRoles.FirstOrDefault()) && x.IsActive);

                if (role != null)
                {
                    userDto.Role = new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name,
                        RoleType = role.RoleType
                    };
                }
            }

            return userDto;
        }

        public async Task<UserDto> Login(string email, string password)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(email) && x.IsActive);

            if (user == null)
            {
                throw new AppUnauthorizedAccessException(UnautorizedType.NOT_FOUND, ObjectType.USER);
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

            if (!result.Succeeded)
            {
                throw new UserManagerException("login", user.Email);
            }

            await AddUserFolder(user);

            var clientRepository = DataContextManager.CreateRepository<IClientRepository>();
            user.Client = await clientRepository.GetById(user.ClientId);

            return MapperFactory.CreateMapper<IUserMapper>().MapToModel(user);
        }


        public async Task Register(RegisterUserDto userDto, RoleType commonRole)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(userDto.Email) && x.IsActive);

            if (user != null)
            {
                throw new AppUnauthorizedAccessException(UnautorizedType.SAME_OBJECT, ObjectType.USER);
            }

            var newUser = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                CreateDate = DateTime.UtcNow,
                ClientId = (await GetClientById(userDto.ClientId)).Id,
                DepartmentId = (await GetDepartmentById(userDto.DepartmentId))?.Id,
                IsActive = true
            };

            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.RoleType == commonRole && x.IsActive);

            if (role == null)
            {
                throw new AppUnauthorizedAccessException(UnautorizedType.NOT_FOUND, ObjectType.ROLE);
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _userManager.CreateAsync(newUser, userDto.Password);

                if (!result.Succeeded)
                {
                    scope.Dispose();
                    throw new UserManagerException("create user", newUser.Email);
                }

                result = await _userManager.AddToRoleAsync(newUser, role.Name);

                if (!result.Succeeded)
                {
                    scope.Dispose();
                    throw new UserManagerException("add to role", newUser.Email);
                }

                scope.Complete();
            }

            await _signInManager.SignInAsync(newUser, false);
            await AddUserFolder(newUser);
        }

        public async Task ResetPassword(ResetPasswordUserDto resetPasswordUserDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(resetPasswordUserDto.Email) && x.IsActive);

            if (user == null)
            {
                throw new AppUnauthorizedAccessException(UnautorizedType.NOT_FOUND, ObjectType.USER);
            }

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, resetPasswordUserDto.Password);
            user.ResetPasswordCode = null;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new UserManagerException("reset password", user.Email);
            }
        }

        public async Task Update(UpdateUserDto userDto, string commonPassword)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userDto.Id && x.IsActive);

            if (user == null)
            {
                throw new NotFoundException("User", userDto.Id);
            }

            var otherUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(userDto.Email) && x.IsActive);

            if (otherUser != null && otherUser.Id != user.Id)
            {
                throw new FoundSameObjectException("User", userDto.UserName);
            }

            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.UpdateDate = DateTime.UtcNow;
            user.ClientId = (await GetClientById(userDto.ClientId)).Id;
            user.DepartmentId = (await GetDepartmentById(userDto.DepartmentId))?.Id;

            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == userDto.RoleId && x.IsActive);

            if (role == null)
            {
                throw new NotFoundDependencyObjectException("Role", userDto.RoleId);
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    scope.Dispose();
                    throw new UserManagerException("update", user.Email);
                }

                try
                {
                    await UpdateUserRole(user, role);
                }
                catch (UserManagerException umex)
                {
                    scope.Dispose();
                    throw new UserManagerException(umex.Message);
                }

                scope.Complete();
            } 
        }

        public async Task Remove(RemoveUserDto userDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userDto.Id && x.IsActive);

            if (user == null)
            {
                throw new NotFoundException("User", userDto.Id);
            }

            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();

            var fileStorages = await fileStorageRepository.GetByUserId(user.Id);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    scope.Dispose();
                    throw new UserManagerException("remove", user.Email);
                }

                scope.Complete();
            }

            fileStorages.SelectMany(s => s.Permissions)
                .Where(x => !x.EndDate.HasValue)
                .ToList()
                .ForEach(perm =>
                {
                    perm.EndDate = DateTime.UtcNow;
                });

            await fileStorageRepository.Update(fileStorages);
        }

        private async Task<Client> GetClientById(int id)
        {
            var clientRepository = DataContextManager.CreateRepository<IClientRepository>();
            var client = await clientRepository.GetById(id);

            if (client == null)
            {
                throw new NotFoundDependencyObjectException("Client", id);
            }

            return client;
        }

        private async Task<Department> GetDepartmentById(int? id)
        {
            Department department = null;

            if (id.HasValue)
            {
                var departmentRepository = DataContextManager.CreateRepository<IDepartmentRepository>();
                department = await departmentRepository.GetById(id.Value);

                if (department == null)
                {
                    throw new NotFoundDependencyObjectException("Department", id.Value);
                }
            }

            return department;
        }

        private async Task AddUserToRole(User user, Role role)
        {
            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                throw new UserManagerException("add to role", user.Email);
            }
        }

        private async Task UpdateUserRole(User user, Role role)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains(role.Name))
            {
                var result = await _userManager.RemoveFromRolesAsync(user, roles);

                if (!result.Succeeded)
                {
                    throw new UserManagerException("Remove from role", user.Email);
                }

                await AddUserToRole(user, role);
            }
        }

        private string GenerateForgotPassowrdCodeToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private async Task AddUserFolder(User user)
        {
            var fileStorageRepository = DataContextManager.CreateRepository<IFileStorageRepository>();

            var userHasFolder = await fileStorageRepository.UserHasFolder(user.Id);

            if (!userHasFolder)
            {
                var newFolder = new FileStorage
                {
                    CreateById = user.Id,
                    CreateDate = DateTime.UtcNow,
                    IsDirectory = true,
                    Name = "My Folder",
                    OwnerId = user.Id,
                    ParentFileStorageId = 1
                };

                await fileStorageRepository.Add(newFolder);
            }
        }

        public async Task<IEnumerable<UserDto>> GetByClientIds(IEnumerable<int> clientIds)
        {
            var users = await _userManager.FindByClientIds(clientIds);
            return MapperFactory.CreateMapper<IUserMapper>().MapCollectionToModel(users);
        }

        public async Task<IEnumerable<UserDto>> GetByIds(IEnumerable<int> userIds)
        {
            var users = await _userManager.FindByIds(userIds);
            return MapperFactory.CreateMapper<IUserMapper>().MapCollectionToModel(users);
        }

        public async Task<IEnumerable<UserDto>> GetByGroupIds(IEnumerable<int> groupIds)
        {
            var userGroupRepository = DataContextManager.CreateRepository<IUserGroupRepository>();

            var groups = await userGroupRepository.GetByIds(groupIds);
            var users = groups.SelectMany(s => s.UsersInGroup).Select(s => s.User).Where(x => x.IsActive).ToList();
            return MapperFactory.CreateMapper<IUserMapper>().MapCollectionToModel(users);
        }
    }
}
