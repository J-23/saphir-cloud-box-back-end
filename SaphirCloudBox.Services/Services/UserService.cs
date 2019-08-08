using Anthill.Common.Data.Contracts;
using Anthill.Common.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaphirCloudBox.Data.Contracts;
using SaphirCloudBox.Data.Contracts.Repositories;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Mappers;
using SaphirCloudBox.Services.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class UserService : AbstractService, IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserService(IUnityContainer container, 
            ISaphirCloudBoxDataContextManager dataContextManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<int>> roleManager) : base(container, dataContextManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task Add(AddUserDto userDto, string password)
        {
            var user = await _userManager.FindByEmailAsync(userDto.Email);

            if (user != null)
            {
                throw new FoundSameObjectException();
            }

            var newUser = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                CreateDate = DateTime.Now,
                ClientId = (await GetClientById(userDto.ClientId)).Id,
                DepartmentId = (await GetDepartmentById(userDto.DepartmentId))?.Id
            };

            var role = await _roleManager.FindByIdAsync(userDto.RoleId.ToString());

            if (role == null)
            {
                throw new NotFoundException();
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _userManager.CreateAsync(newUser, password);

                if (!result.Succeeded)
                {
                    scope.Dispose();
                    throw new AddException();
                }
                
                try
                {
                    await AddUserToRole(newUser, role);
                }
                catch (RoleAddException)
                {
                    scope.Dispose();
                    throw new AddException();
                }

                scope.Complete();
            }

        }

        public async Task<string> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new ArgumentException(nameof(user));
            }

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            var users = await _userManager.Users.OrderByDescending(ord => ord.CreateDate).ThenByDescending(ord => ord.UpdateDate).ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var userDto = MapperFactory.CreateMapper<IUserMapper>().MapToModel(user);
                userDto.Role = roles.Where(x => userRoles.Contains(x.Name))
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name
                    })
                    .FirstOrDefault();

                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<UserDto> GetByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new ArgumentException(nameof(user));
            }

            return MapperFactory.CreateMapper<IUserMapper>().MapToModel(user);
        }

        public async Task<UserDto> GetById(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new ArgumentException(nameof(user));
            }

            return MapperFactory.CreateMapper<IUserMapper>().MapToModel(user);
        }

        public async Task<UserDto> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new ArgumentException(nameof(user));
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException();
            }

            return MapperFactory.CreateMapper<IUserMapper>().MapToModel(user);
        }

        public async Task Register(RegisterUserDto userDto)
        {
            var user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                CreateDate = DateTime.Now,
                ClientId = userDto.ClientId
            };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException();
            }

            await _signInManager.SignInAsync(user, false);
        }

        public async Task ResetPassword(ResetPasswordUserDto resetPasswordUserDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordUserDto.Email);

            if (user == null)
            {
                throw new ArgumentException(nameof(user));
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordUserDto.Code, resetPasswordUserDto.Password);

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException();
            }
        }

        public async Task Update(UpdateUserDto userDto, string commonPassword)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

            if (user == null)
            {
                throw new NotFoundException();
            }

            var otherUser = await _userManager.FindByEmailAsync(userDto.Email);

            if (otherUser != null && otherUser.Id != user.Id)
            {
                throw new FoundSameObjectException();
            }

            user.UserName = userDto.UserName;
            user.UpdateDate = DateTime.Now;
            user.ClientId = (await GetClientById(userDto.ClientId)).Id;
            user.DepartmentId = (await GetDepartmentById(userDto.DepartmentId))?.Id;

            var role = await _roleManager.FindByIdAsync(userDto.RoleId.ToString());

            if (role == null)
            {
                throw new NotFoundException();
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    scope.Dispose();
                    throw new UpdateException();
                }

                try
                {
                    await UpdateUserRole(user, role);
                }
                catch (NotFoundException)
                {
                    scope.Dispose();
                    throw new UpdateException();
                }
                catch (RemoveException)
                {
                    scope.Dispose();
                    throw new UpdateException();
                }
                catch (RoleAddException)
                {
                    scope.Dispose();
                    throw new UpdateException();
                }

                scope.Complete();
            } 
        }

        public async Task Remove(RemoveUserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

            if (user == null)
            {
                throw new NotFoundException();
            }

            var roles = await _userManager.GetRolesAsync(user);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _userManager.RemoveFromRolesAsync(user, roles);

                if (!result.Succeeded)
                {
                    scope.Dispose();
                    throw new RemoveException();
                }

                result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    scope.Dispose();
                    throw new RemoveException();
                }

                scope.Complete();
            } 
        }

        private async Task<Client> GetClientById(int id)
        {
            var clientRepository = DataContextManager.CreateRepository<IClientRepository>();
            var client = await clientRepository.GetById(id);

            if (client == null)
            {
                throw new NotFoundDependencyObjectException();
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
                    throw new NotFoundDependencyObjectException();
                }
            }

            return department;
        }

        private async Task AddUserToRole(User user, IdentityRole<int> role)
        {
            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                throw new RoleAddException();
            }
        }

        private async Task UpdateUserRole(User user, IdentityRole<int> role)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains(role.Name))
            {
                var result = await _userManager.RemoveFromRolesAsync(user, roles);

                if (!result.Succeeded)
                {
                    throw new RemoveException();
                }

                await AddUserToRole(user, role);
            }
        }
    }
}
