using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Infractructure
{
    public class AppSettings
    {
        public string FrontEndUrl { get; set; }

        public string CommonPassword { get; set; }

        public string CommonRole { get; set; }

        public string FileManagerUrlPart { get; set; }

        public string SharedWithMeUrlPart { get; set; }
    }
}
