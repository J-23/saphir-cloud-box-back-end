﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SaphirCloudBox.Host.Helpers;
using SaphirCloudBox.Host.Infractructure;
using SaphirCloudBox.Host.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Exceptions;
using SaphirCloudBox.Services.Contracts.Services;

namespace SaphirCloudBox.Host.Controllers
{
    [Route("api/account")]
    [ApiController]
    [AllowAnonymous]
    [EnableCors("CorsPolicy")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;

        private readonly AppSettings _appSettings;

        public AccountController(IUserService userService, IEmailSender emailSender, AppSettings appSettings)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));

            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody]LoginModel model)
        {
            var identity = await GetIdentity(model.Email, model.Password);

            var now = DateTime.UtcNow;

            var newToken = GenerateToken(identity.Claims, now);

            var response = new
            {
                access_token = newToken,
                expires_date = now.Add(TimeSpan.FromMinutes(Constants.LIFETIME))
            };

            return Ok(response);
        }


        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody]RegisterUserDto userDto)
        {
            await _userService.Register(userDto, _appSettings.CommonRole);

            var identity = await GetIdentity(userDto.Email, userDto.Password);

            var now = DateTime.UtcNow;
            var newToken = GenerateToken(identity.Claims, now);

            var response = new
            {
                access_token = newToken,
                expires_date = now.Add(TimeSpan.FromMinutes(Constants.LIFETIME))
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword([FromBody]ForgotPasswordModel model)
        {
            var user = await _userService.GetByEmail(model.Email);
            var code = await _userService.ForgotPassword(model.Email);

            var resetPasswordLink = _appSettings.FrontEndUrl + String.Format(Constants.NotificationMessages.ResetPasswordUrl, code);
            var message = String.Format(Constants.NotificationMessages.ForgotPasswordMessage, user.UserName, _appSettings.FrontEndUrl, resetPasswordLink);

            await _emailSender.Send(EmailType.Auth, new MailAddress(user.Email, user.UserName), Constants.NotificationMessages.ForgotPasswordSubject, message);

            var response = new
            {
                forgot_password_token = code
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword([FromBody]ResetPasswordUserDto resetPassword)
        {
            await _userService.ResetPassword(resetPassword);
            return Ok();
        }

        [HttpGet]
        [Route("user")]
        [Authorize(Policy = "Bearer")]
        public async Task<ActionResult> GetUser()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type.Contains("UserId"));

            if (userId == null)
            {
                return Ok(null);
            }

            var user = await _userService.GetById(Int32.Parse(userId.Value));
            return Ok(user);
        }

        private async Task<ClaimsIdentity> GetIdentity(string email, string password)
        {
            var user = await _userService.Login(email, password);

            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("ClientId", user.Client.Id.ToString())
                };

            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        private string GenerateToken(IEnumerable<Claim> claims, DateTime now)
        {
            var jwt = new JwtSecurityToken(
                    issuer: Constants.ISSUER,
                    audience: Constants.AUDIENCE,
                    notBefore: now,
                    claims: claims,
                    expires: now.Add(TimeSpan.FromMinutes(Constants.LIFETIME)),
                    signingCredentials: new SigningCredentials(Constants.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}