using SaphirCloudBox.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Services.Contracts.Services
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAll();

        Task Add(AddClientDto clientDto);

        Task Update(UpdateClientDto clientDto);

        Task Remove(RemoveClientDto clientDto);
    }
}
