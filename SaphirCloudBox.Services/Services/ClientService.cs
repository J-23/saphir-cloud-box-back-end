using Anthill.Common.Data.Contracts;
using Anthill.Common.Services;
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
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class ClientService : AbstractService, IClientService
    {
        public ClientService(IUnityContainer container, ISaphirCloudBoxDataContextManager dataContextManager) : base(container, dataContextManager)
        {
        }

        public async Task Add(AddClientDto clientDto)
        {
            var clientRepository = DataContextManager.CreateRepository<IClientRepository>();

            var client = await clientRepository.GetByName(clientDto.Name);

            if (client != null)
            {
                throw new FoundSameObjectException("Client", clientDto.Name);
            }

            var newClient = new Client
            {
                Name = clientDto.Name,
                CreateDate = DateTime.Now
            };

            await clientRepository.Add(newClient);
        }

        public async Task<IEnumerable<ClientDto>> GetAll()
        {
            var clientRepository = DataContextManager.CreateRepository<IClientRepository>();

            var clients = await clientRepository.GetAll();

            return MapperFactory.CreateMapper<IClientMapper>().MapCollectionToModel(clients);
        }

        public async Task Remove(RemoveClientDto clientDto)
        {
            var clientRepository = DataContextManager.CreateRepository<IClientRepository>();

            var client = await clientRepository.GetById(clientDto.Id);

            if (client == null)
            {
                throw new NotFoundException("Client", clientDto.Id);
            }

            if (client.Departments.Count() > 0 || client.Users.Count() > 0)
            {
                throw new ExistDependencyException("Client", clientDto.Id, new List<string> { "Departments", "Users" });
            }

            await clientRepository.Remove(client);
        }

        public async Task Update(UpdateClientDto clientDto)
        {
            var clientRepository = DataContextManager.CreateRepository<IClientRepository>();

            var client = await clientRepository.GetById(clientDto.Id);

            if (client == null)
            {
                throw new NotFoundException("Client", clientDto.Id);
            }

            var otherClient = await clientRepository.GetByName(clientDto.Name);

            if (otherClient != null && otherClient.Id != clientDto.Id)
            {
                throw new FoundSameObjectException("Client", clientDto.Name);
            }

            client.Name = clientDto.Name;
            client.UpdateDate = DateTime.Now;

            await clientRepository.Update(client);
        }
    }
}
