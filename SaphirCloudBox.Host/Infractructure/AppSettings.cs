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
    }
}
