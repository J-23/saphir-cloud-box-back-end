using SaphirCloudBox.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Contracts.Services
{
    public interface IUserService
    {
        Task Register(RegisterUserDto userDto, string commonRole);

        Task<UserDto> Login(string email, string password);

        Task<string> ForgotPassword(string email);

        Task ResetPassword(ResetPasswordUserDto resetPasswordUserDto);

        Task<UserDto> GetById(int userId);

        Task<UserDto> GetByEmail(string email);

        Task<IEnumerable<UserDto>> GetAll(int userId, int clientId);

        Task Add(AddUserDto userDto, string password);

        Task Update(UpdateUserDto userDto, string commonPassword);

        Task Remove(RemoveUserDto userDto);

        Task<IEnumerable<UserDto>> GetByClientIds(IEnumerable<int> clientIds);

        Task<IEnumerable<UserDto>> GetByIds(IEnumerable<int> userIds);

        Task<IEnumerable<UserDto>> GetByGroupIds(IEnumerable<int> groupIds);
    }
}
