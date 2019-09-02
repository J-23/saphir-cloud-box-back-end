using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaphirCloudBox.Services.Contracts.Dtos.UserGroup;
using SaphirCloudBox.Services.Contracts.Services;

namespace SaphirCloudBox.Host.Controllers
{
    [Route("api/user-group")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(Policy = "Bearer")]
    public class UserGroupController : BaseController
    {
        private readonly IUserGroupService _userGroupService;

        public UserGroupController(ILogService logService,
            IUserGroupService userGroupService) 
            : base(logService)
        {
            _userGroupService = userGroupService ?? throw new ArgumentNullException(nameof(userGroupService));
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult> GetUserGroups()
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var groupDtos = await _userGroupService.GetGroups(UserId);
            return Ok(groupDtos);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> AddGroup([FromBody]AddUserGroupDto groupDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _userGroupService.Add(groupDto, UserId);
            return Ok();
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> UpdateGroup([FromBody]UpdateUserGroupDto groupDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _userGroupService.Update(groupDto, UserId);
            return Ok();
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult> RemoveGroup([FromBody]RemoveUserGroupDto groupDto)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            await _userGroupService.Remove(groupDto, UserId);
            return Ok();
        }
    }
}