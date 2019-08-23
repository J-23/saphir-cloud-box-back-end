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
        public EmailSender()
        {
        }

        public async Task Send(string senderHost, int senderPort, string senderEmail, string senderPassword, 
            MailAddress recipient, string subject, string message, string fileName = "", string fileContent = "")
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Host = senderHost;
                smtpClient.Port = senderPort;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);

                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress(senderEmail);
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
    }
}
