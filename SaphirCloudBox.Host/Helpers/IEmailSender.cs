using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Helpers
{
    public interface IEmailSender
    {
        Task Send(EmailType emailType, MailAddress recipient, string subject, string message, string fileName = "", string fileContent = "");
    }

    public enum EmailType
    {
        Auth,
        TechSupport,
        Notification
    }
}
