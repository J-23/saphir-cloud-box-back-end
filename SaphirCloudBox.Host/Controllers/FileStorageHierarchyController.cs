using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaphirCloudBox.Services.Contracts.Services;

namespace SaphirCloudBox.Host.Controllers
{
    [Route("api/hierarchy")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(Policy = "Bearer")]
    public class FileStorageHierarchyController : BaseController
    {
        private readonly IFileStorageHierarchyService _fileStorageHierarchyService;

        public FileStorageHierarchyController(ILogService logService,
            IFileStorageHierarchyService fileStorageHierarchyService) 
            : base(logService)
        {
            _fileStorageHierarchyService = fileStorageHierarchyService ?? throw new ArgumentNullException(nameof(fileStorageHierarchyService));
        }

        [HttpGet]
        [Route("{parentId}")]
        public async Task<ActionResult> GetByParentId(int parentId)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var folders = await _fileStorageHierarchyService.GetByParentId(parentId, UserId, ClientId);
            return Ok(folders);
        }

        [HttpGet]
        [Route("by-child/{childId}")]
        public async Task<ActionResult> Get(int childId)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var folder = await _fileStorageHierarchyService.GetByChildId(childId, UserId, ClientId);
            return Ok(folder);
        }
    }
}