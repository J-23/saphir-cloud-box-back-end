using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaphirCloudBox.Host.Helpers;
using SaphirCloudBox.Host.Infractructure;
using SaphirCloudBox.Host.Models;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Services;

namespace SaphirCloudBox.Host.Controllers
{
    [Route("api/feedback")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize(Policy = "Bearer")]
    public class FeedbackController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;

        private readonly EmailSettings _emailSettings;

        public FeedbackController(ILogService logService, 
            IUserService userService,
            IEmailSender emailSender,
            EmailSettings emailSettings) 
            : base(logService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _emailSettings = emailSettings ?? throw new ArgumentNullException(nameof(emailSettings));
        }

        [HttpPost]
        [Route("send")]
        public async Task<ActionResult> SendMessage([FromBody]SendMessageModel model)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            var user = await _userService.GetByEmail(model.UserEmail);

            var subject = String.Format(Constants.NotificationMessages.TechnicalSupportSubject, model.UserName);
            var message = String.Format(Constants.NotificationMessages.TechnicalSupportMessage, model.UserName, user.Email, model.Theme, model.Message);

            await _emailSender.Send(EmailType.TechSupport, new MailAddress(_emailSettings.TechSupportEmail), subject, message, model.FileName, model.FileContent);

            return Ok();
        }
    }
}