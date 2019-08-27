using SaphirCloudBox.Host.Infractructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Helpers
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings ?? throw new ArgumentNullException(nameof(emailSettings));
        }

        public async Task Send(EmailType emailType, MailAddress recipient, string subject, string message, string fileName = "", string fileContent = "")
        {
            var settings = GetSettings(emailType);

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Host = settings.SmtpHost;
                smtpClient.Port = settings.SmtpPort;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(settings.Email, settings.Password);

                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(settings.Email);
                    msg.To.Add(recipient);
                    msg.Subject = subject;
                    msg.Body = message;

                    if (!String.IsNullOrEmpty(fileName) && !String.IsNullOrEmpty(fileContent))
                    {
                        var bytes = Convert.FromBase64String(fileContent);

                        using (var stream = new MemoryStream(bytes))
                        {
                            msg.Attachments.Add(new Attachment(stream, fileName));
                            await smtpClient.SendMailAsync(msg);
                        }
                    }
                    else
                    {
                        await smtpClient.SendMailAsync(msg);
                    }
                }
            }
        }

        private (string SmtpHost, int SmtpPort, string Email, string Password) GetSettings(EmailType emailType)
        {
            switch (emailType)
            {
                case EmailType.Auth:
                    return (_emailSettings.AuthSmtpHost, _emailSettings.AuthSmtpPort, _emailSettings.AuthSenderEmail, _emailSettings.AuthSenderPassword);
                case EmailType.TechSupport:
                    return (_emailSettings.TechSupportSmtpHost, _emailSettings.TechSupportSmtpPort, _emailSettings.TechSupportEmail, _emailSettings.TechSupportPassword);
                case EmailType.Notification:
                    return (_emailSettings.NotificationSmtpHost, _emailSettings.NotificationSmtpPort, _emailSettings.NotificationEmail, _emailSettings.NotificationPassword);
                default:
                    throw new ArgumentException(nameof(emailType));
            }
        }
    }
}
