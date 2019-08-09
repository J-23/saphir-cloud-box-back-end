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

        public ClientController(IClientService clientService)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
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
            try
            {
                await _clientService.Add(clientDto);
                return Ok();
            }
            catch (FoundSameObjectException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UdpateClient([FromBody]UpdateClientDto clientDto)
        {
            try
            {
                await _clientService.Update(clientDto);
                return Ok();
            }
            catch (NotFoundException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (FoundSameObjectException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveClient([FromBody]RemoveClientDto clientDto)
        {
            try
            {
                await _clientService.Remove(clientDto);
                return Ok();
            }
            catch (NotFoundException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (RemoveException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.REMOVE_ERROR.ToString());
            }
        }
    }
}