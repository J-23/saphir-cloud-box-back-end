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
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogService _logService;

        public ClientController(IClientService clientService, ILogService logService)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        [HttpGet]
        [Route("list")]
        [AllowAnonymous]
        public async Task<ActionResult> GetClientList()
        {
            var clients = await _clientService.GetAll();
            return Ok(clients);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> AddClient([FromBody]AddClientDto clientDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _clientService.Add(clientDto);
            await _logService.Add(Enums.LogType.Create, $"Client with name = {clientDto.Name} was created successfully by user = {userId}");
            return Ok();
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateClient([FromBody]UpdateClientDto clientDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _clientService.Update(clientDto);
            await _logService.Add(Enums.LogType.Update, $"Client with id = {clientDto.Id} was updated successfully by user = {userId}");
            return Ok();
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveClient([FromBody]RemoveClientDto clientDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _clientService.Remove(clientDto);
            await _logService.Add(Enums.LogType.Remove, $"Client with id = {clientDto.Id} was removed successfully by user = {userId}");
            return Ok();
        }
    }
}