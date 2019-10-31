using Anthill.Common.Data.Contracts;
using Anthill.Common.Services;
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
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace SaphirCloudBox.Services.Services
{
    public class ClientService : AbstractService, IClientService
    {
        private readonly IUserService _userService;

        public ClientService(IUnityContainer container, 
            ISaphirCloudBoxDataContextManager dataContextManager,
            IUserService userService) : base(container, dataContextManager)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
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
                CreateDate = DateTime.UtcNow,
                IsActive = true
            };

            await clientRepository.Add(newClient);
        }

        public async Task<IEnumerable<ClientDto>> GetAll()
        {
            var clientRepository = DataContextManager.CreateRepository<IClientRepository>();

            var clients = await clientRepository.GetAll();

            return MapperFactory.CreateMapper<IClientMapper>().MapCollectionToModel(clients);
        }

        public async Task<IEnumerable<ClientDto>> GetAll(int userId, int clientId)
        {
            var user = await _userService.GetById(userId);

            IEnumerable<Client> clients = new List<Client>();

            if (user.Role.RoleType == RoleType.SuperAdmin)
            {
                var clientRepository = DataContextManager.CreateRepository<IClientRepository>();
                clients = await clientRepository.GetAll();
            }

            return MapperFactory.CreateMapper<IClientMapper>().MapCollectionToModel(clients);
        }

        public async Task<IEnumerable<ClientDto>> GetByUserId(int userId)
        {
            var clientRepository = DataContextManager.CreateRepository<IClientRepository>();

            var user = await _userService.GetById(userId);

            var clients = new List<Client>();

            switch (user.Role.RoleType)
            {
                case Enums.RoleType.SuperAdmin:
                    clients = (await clientRepository.GetAll()).ToList();
                    break;
                case Enums.RoleType.ClientAdmin:
                    clients.Add(await clientRepository.GetById(user.Client.Id));
                    break;
            }

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

            if (client.Departments.Where(x => x.IsActive).Count() > 0 || client.Users.Where(x => x.IsActive).Count() > 0)
            {
                throw new ExistDependencyException("Client", clientDto.Id, new List<string> { "Departments", "Users" });
            }

            client.IsActive = false;
            await clientRepository.Update(client);
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
            client.UpdateDate = DateTime.UtcNow;

            await clientRepository.Update(client);
        }
    }
}
