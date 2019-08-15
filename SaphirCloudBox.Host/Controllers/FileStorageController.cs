using System;
using System.Collections.Generic;
using System.IO;
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
    [Route("api/file-storage")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(Policy = "Bearer")]
    public class FileStorageController : BaseController
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogService _logService;

        public FileStorageController(IFileStorageService fileStorageService, ILogService logService)
        {
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        [HttpGet]
        [Route("{parentId}")]
        public async Task<ActionResult> GetFolders(int parentId = 0)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));
            var clientIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("ClientId"));

            if (userIdClaim == null || clientIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);
            var clientId = Convert.ToInt32(clientIdClaim.Value);

            try
            {
                var fileStorages = await _fileStorageService.GetByParentId(parentId, userId, clientId);

                return Ok(fileStorages);
            }
            catch (NotFoundException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"Directory with Id = {parentId} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch(Exception ex)
            {
                await _logService.Add(Enums.LogType.NotFound, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("add/folder")]
        public async Task<ActionResult> AddFolder([FromBody]AddFolderDto folderDto)
        {
            var userId = GetUser();
            var clientId = GetClient();

            if (!userId.HasValue || !clientId.HasValue)
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.AddFolder(folderDto, userId.Value, clientId.Value);
                await _logService.Add(Enums.LogType.Create, $"Directory with Id = {folderDto.Name} was created successfully by user = {userId.Value}");
                return Ok();
            }
            catch (NotFoundException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"Directory with Id = {folderDto.ParentId} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (FoundSameObjectException)
            {
                await _logService.Add(Enums.LogType.SameObject, $"Failed to create directory = {folderDto.Name}. Directory already exists with that name");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (Exception ex)
            {
                await _logService.Add(Enums.LogType.NotFound, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("remove/folder")]
        public async Task<ActionResult> RemoveFolder([FromBody]RemoveFileStorageDto folderDto)
        {
            var userId = GetUser();
            var clientId = GetClient();

            if (!userId.HasValue || !clientId.HasValue)
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.RemoveFolder(folderDto, userId.Value, clientId.Value);
                await _logService.Add(Enums.LogType.Create, $"Directory with Id = {folderDto.Id} was removed successfully by user = {userId.Value}");
                return Ok();
            }
            catch (NotFoundException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"Directory with Id = {folderDto.Id} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (Exception ex)
            {
                await _logService.Add(Enums.LogType.NotFound, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("update/folder")]
        public async Task<ActionResult> UpdateFolder([FromBody]UpdateFolderDto folderDto)
        {
            var userId = GetUser();
            var clientId = GetClient();

            if (!userId.HasValue || !clientId.HasValue)
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.UpdateFolder(folderDto, userId.Value, clientId.Value);
                await _logService.Add(Enums.LogType.Create, $"Directory with Id = {folderDto.Id} was updated successfully by user = {userId.Value}");
                return Ok();
            }
            catch (NotFoundException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"Directory with Id = {folderDto.Id} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (FoundSameObjectException)
            {
                await _logService.Add(Enums.LogType.SameObject, $"Failed to update directory = {folderDto.Name}. Directory already exists with that name");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (Exception ex)
            {
                await _logService.Add(Enums.LogType.NotFound, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        [HttpPost]
        [Route("add/file")]
        public async Task<ActionResult> AddFile([FromBody]AddFileDto fileDto)
        {
            var userId = GetUser();
            var clientId = GetClient();

            if (!userId.HasValue || !clientId.HasValue)
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.AddFile(fileDto, userId.Value, clientId.Value);
                await _logService.Add(Enums.LogType.Create, $"File = {fileDto.Name} was created successfully by user = {userId.Value}");
                return Ok();
            }
            catch (NotFoundException)
            {
                await _logService.Add(Enums.LogType.NotFound, $"File storage with Id = {fileDto.ParentId} not found");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch (FoundSameObjectException)
            {
                await _logService.Add(Enums.LogType.SameObject, $"Failed to create file = {fileDto.Name}. File already exists with that name");
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SAME_NAME.ToString());
            }
            catch (Exception ex)
            {
                await _logService.Add(Enums.LogType.NotFound, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }

        public async Task<ActionResult> RemoveFile([FromBody]RemoveFileStorageDto fileDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            //await _fileStorageService.RemoveFile(fileDto, userId);
            return Ok();
        }

        [HttpGet]
        [Route("file/{id}")]
        public async Task<ActionResult> DownloadFile(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            var fileDto = await _fileStorageService.GetFileById(id, userId);
            var contentType = Constants.Extension.TYPES[Path.GetExtension(fileDto.Name).ToLowerInvariant()];

            using (WebClient webClient = new WebClient())
            {
                return File(fileDto.Buffer, contentType, fileDto.Name);
            }
        }
    }
}