using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Infractructure
{
    public class EmailSettings
    {
        public string AuthSmtpHost { get; set; }

        public int AuthSmtpPort { get; set; }

        public string AuthSenderEmail { get; set; }

        public string AuthSenderPassword { get; set; }

        public string TechSupportSmtpHost { get; set; }

        public int TechSupportSmtpPort { get; set; }

        public string TechSupportEmail { get; set; }

        public string TechSupportPassword { get; set; }

        public string NotificationSmtpHost { get; set; }

        public int NotificationSmtpPort { get; set; }

        public string NotificationEmail { get; set; }

        public string NotificationPassword { get; set; }
    }
}