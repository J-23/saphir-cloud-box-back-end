using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaphirCloudBox.Enums;
using SaphirCloudBox.Services.Contracts.Services;

namespace SaphirCloudBox.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly ILogService _logService;

        protected int ClientId;
        protected int UserId;

        public BaseController(ILogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        protected bool IsAvailableOperation()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            var clientIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("ClientId"));

            if (clientIdClaim == null || userIdClaim == null)
            {
                return false;
            }

            UserId = Convert.ToInt32(userIdClaim.Value);
            ClientId = Convert.ToInt32(clientIdClaim.Value);

            return true;
        }

        protected async Task AddLog(LogType logType, string message)
        {
            await _logService.Add(logType, message);
        }
    }
}