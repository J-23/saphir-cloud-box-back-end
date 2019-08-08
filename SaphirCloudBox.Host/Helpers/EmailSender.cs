using SaphirCloudBox.Host.Infractructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Helpers
{
    public class EmailSender : IEmailSender
    {
        private readonly AppSettings _appSettings;

        public EmailSender(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public async Task Send(MailAddress recipient, string subject, string message)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Host = _appSettings.SmtpHost;
                smtpClient.Port = _appSettings.SmtpPort;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_appSettings.SenderEmail, _appSettings.SenderPassword);

                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(_appSettings.SenderEmail);
                    msg.To.Add(recipient);
                    msg.Subject = subject;
                    msg.Body = message;

                    await smtpClient.SendMailAsync(msg);
                }
            }
        }
    }
}
