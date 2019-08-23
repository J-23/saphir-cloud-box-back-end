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

        private readonly AppSettings _appSettings;

        public FeedbackController(ILogService logService, 
            IUserService userService,
            IEmailSender emailSender,
            AppSettings appSettings) 
            : base(logService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        [HttpPost]
        [Route("send")]
        public async Task<ActionResult> SendMessage([FromBody]SendMessageModel model)
        {
            if (!IsAvailableOperation())
            {
                return BadRequest();
            }

            try
            {
                var user = await _userService.GetByEmail(model.UserEmail);

                var subject = String.Format(Constants.NotificationMessages.TechnicalSupportSubject, model.UserName);
                var message = String.Format(Constants.NotificationMessages.TechnicalSupportMessage, model.UserName, user.Email, model.Theme, model.Message);

                await _emailSender.Send(_appSettings.TechSupportHost, _appSettings.TechSupportPort, _appSettings.TechSupportEmail, _appSettings.TechSupportPassword, 
                    new MailAddress(_appSettings.TechSupportEmail), subject, message, model.FileName, model.FileContent);

                return Ok();
            }
            catch (NotFoundException)
            {
                await AddLog(Enums.LogType.NotFound, LogMessage.CreateNotFoundByEmailMessage(LogMessage.UserEntityName, model.UserEmail));
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.NOT_FOUNT.ToString());
            }
            catch(Exception ex)
            {
                await AddLog(Enums.LogType.Error, ex.Message);
                return StatusCode((int)HttpStatusCode.Forbidden, ResponseMessage.SERVER_ERROR.ToString());
            }
        }
    }
}