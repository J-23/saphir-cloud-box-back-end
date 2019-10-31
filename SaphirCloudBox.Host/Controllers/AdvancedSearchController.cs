using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Services;

namespace SaphirCloudBox.Host.Controllers
{
    [Route("api/advanced-search")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(Policy = "Bearer")]
    public class AdvancedSearchController : BaseController
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IClientService _clientService;
        private readonly IDepartmentService _departmentService;
        private readonly IUserService _userService;
        private readonly IUserGroupService _userGroupService;
        private readonly IFileStorageHierarchyService _fileStorageHierarchyService;

        public AdvancedSearchController(ILogService logService,
            IFileStorageService fileStorageService,
            IClientService clientService,
            IDepartmentService departmentService,
            IUserService userService,
            IUserGroupService userGroupService,
            IFileStorageHierarchyService fileStorageHierarchyService) 
            : base(logService)
        {
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userGroupService = userGroupService ?? throw new ArgumentNullException(nameof(userGroupService));
            _fileStorageHierarchyService = fileStorageHierarchyService ?? throw new ArgumentNullException(nameof(fileStorageHierarchyService));
        }

        [HttpPost]
        [Route("get")]
        public async Task<ActionResult> SearchByFileStorage([FromBody]AdvancedSearchDto advancedSearchDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var fileStorages = await _fileStorageService.Search(advancedSearchDto, UserId, ClientId);

            return Ok(fileStorages);
        }

        [HttpGet]
        [Route("clients")]
        public async Task<ActionResult> GetClients()
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var clients = await _clientService.GetAll(UserId, ClientId);
            return Ok(clients);
        }

        [HttpGet]
        [Route("departments")]
        public async Task<ActionResult> GetDepartments()
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var departments = await _departmentService.GetAll(UserId, ClientId);
            return Ok(departments);
        }

        [HttpGet]
        [Route("users")]
        public async Task<ActionResult> GetUsers()
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var users = await _userService.GetAll(UserId, ClientId);
            return Ok(users);
        }

        [HttpGet]
        [Route("groups")]
        public async Task<ActionResult> GetGroups()
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var groups = await _userGroupService.GetAll(UserId, ClientId);
            return Ok(groups);
        }

        [HttpGet]
        [Route("storage/{id}")]
        public async Task<ActionResult> Get(int id)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var folder = await _fileStorageHierarchyService.GetByChildId(id, UserId, ClientId);
            return Ok(folder);
        }
    }
}