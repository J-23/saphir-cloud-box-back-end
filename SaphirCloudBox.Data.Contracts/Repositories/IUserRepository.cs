using Anthill.Common.Data.Contracts;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Contracts.Repositories
{
    public interface IUserRepository: IRepository
    {
        Task<IEnumerable<User>> GetUsersByClientId(int clientId);
    }
}
