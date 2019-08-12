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
    public class FileStorageController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        public FileStorageController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        }

        [HttpGet]
        [Route("{parentId}")]
        public async Task<ActionResult> GetFolders(int parentId = 0)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            var folders = await _fileStorageService.GetByParentId(parentId, userId);

            return Ok(folders);
        }

        [HttpPost]
        [Route("add/folder")]
        public async Task<ActionResult> AddFolder([FromBody]AddFolderDto folderDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            try
            {
                await _fileStorageService.AddFolder(folderDto, userId);
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
        [Route("add/file")]
        public async Task<ActionResult> AddFile([FromBody]AddFileDto fileDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            try
            {
                await _fileStorageService.AddFile(fileDto, userId);
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

        public async Task<ActionResult> RemoveFile([FromBody]RemoveFileDto fileDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            await _fileStorageService.RemoveFile(fileDto, userId);
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