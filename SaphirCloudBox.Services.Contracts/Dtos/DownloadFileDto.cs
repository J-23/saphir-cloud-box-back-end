using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class DownloadFileDto
    {
        public string Name { get; set; }

        public byte[] Buffer { get; set; }
    }
}
