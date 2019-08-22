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

            try
            {
                await _clientService.Add(clientDto);
                await _logService.Add(Enums.LogType.Create, $"Client with name = {clientDto.Name} was created successfully by user = {userId}");
                return Ok();
            }
            catch (FoundSameObjectException)
            {
                await _logService.Add(Enums.LogType.SameObject, $"Failed to create client with name = {clientDto.Name}. Client already exists with that name");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch(Exception ex)
            {
                await _logService.Add(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
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

            try
            {
                await _clientService.Update(clientDto);
                await _logService.Add(Enums.LogType.Update, $"Client with id = {clientDto.Id} was updated successfully by user = {userId}");
                return Ok();
            }
            catch (NotFoundException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"Client with Id = {clientDto.Id} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND.ToString());
            }
            catch (FoundSameObjectException)
            {
                await _logService.Add(Enums.LogType.SameObject, $"Failed to update client with id = {clientDto.Id} and name = {clientDto.Name}. Client already exists with that name");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (Exception ex)
            {
                await _logService.Add(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
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

            try
            {
                await _clientService.Remove(clientDto);
                await _logService.Add(Enums.LogType.Remove, $"Client with id = {clientDto.Id} was removed successfully by user = {userId}");
                return Ok();
            }
            catch (NotFoundException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"Client with Id = {clientDto.Id} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUND.ToString());
            }
            catch (RemoveException)
            {
                await _logService.Add(Enums.LogType.Error, $"Client with Id = {clientDto.Id} has departments and / or users");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.REMOVE_ERROR.ToString());
            }
            catch (Exception ex)
            {
                await _logService.Add(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }
    }
}