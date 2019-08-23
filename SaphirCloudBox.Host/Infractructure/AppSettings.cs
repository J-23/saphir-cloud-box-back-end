using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Infractructure
{
    public class AppSettings
    {
        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public string SenderEmail { get; set; }

        public string SenderPassword { get; set; }

        public string FrontEndUrl { get; set; }

        public string CommonPassword { get; set; }

        public string CommonRole { get; set; }

        public string TechSupportEmail { get; set; }

        public string TechSupportPassword { get; set; }

        public int TechSupportPort { get; set; }

        public string TechSupportHost { get; set; }
    }
}
