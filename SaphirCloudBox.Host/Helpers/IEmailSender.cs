using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Helpers
{
    public interface IEmailSender
    {
        Task Send(MailAddress recipient, string subject, string message);
    }
}
