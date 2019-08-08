using Anthill.Common.Data.Contracts;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Data.Contracts.Repositories
{
    public interface IClientRepository: IRepository
    {
        Task<Client> GetById(int clientId);

        Task<Client> GetByName(string clientName);

        Task Add(Client client);

        Task Update(Client client);

        Task Remove(Client client);

        Task<IEnumerable<Client>> GetAll();
    }
}
