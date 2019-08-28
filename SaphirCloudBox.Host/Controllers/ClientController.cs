using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaphirCloudBox.Host.Infractructure;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Services;

namespace SaphirCloudBox.Host.Controllers
{
    [Route("api/client")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(Policy = "Bearer")]
    public class ClientController : BaseController
    {
        private readonly IClientService _clientService;
        private readonly ILogService _logService;

        public ClientController(IClientService clientService, ILogService logService): base(logService)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        [HttpGet]
        [Route("list")]
        [AllowAnonymous]
        public async Task<ActionResult> GetClientList()
        {
            var userId = GetUserId();

            IEnumerable<ClientDto> clients = new List<ClientDto>();

            if (userId.HasValue)
            {
                clients = await _clientService.GetByUserId(userId.Value);
            }
            else
            {
                clients = await _clientService.GetAll();
            }

            return Ok(clients);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> AddClient([FromBody]AddClientDto clientDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _clientService.Add(clientDto);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByNameMessage(LogMessage.ClientEntityName, clientDto.Name, LogMessage.CreateAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateClient([FromBody]UpdateClientDto clientDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _clientService.Update(clientDto);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.ClientEntityName, clientDto.Id, LogMessage.UpdateAction, UserId));
            return Ok();
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveClient([FromBody]RemoveClientDto clientDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _clientService.Remove(clientDto);
            AddLog(Enums.LogType.Create, LogMessage.CreateSuccessByIdMessage(LogMessage.ClientEntityName, clientDto.Id, LogMessage.RemoveAction, UserId));
            return Ok();
        }
    }
}