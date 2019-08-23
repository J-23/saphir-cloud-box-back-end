using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Helpers
{
    public interface IEmailSender
    {
        Task Send(string senderHost, int senderPort, string senderEmail, string senderPassword, 
            MailAddress recipient, string subject, string message, string fileName = "", string fileContent = "");
    }
}
