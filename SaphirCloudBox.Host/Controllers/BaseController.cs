using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SaphirCloudBox.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected int? GetUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userIdClaim == null)
            {
                return null;
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            return userId;
        }

        protected int? GetClient()
        {
            var clientIdClaim = User.Claims.FirstOrDefault(x => x.Type.Contains("ClientId"));

            if (clientIdClaim == null)
            {
                return null;
            }

            var clientId = Convert.ToInt32(clientIdClaim.Value);

            return clientId;
        }
    }
}