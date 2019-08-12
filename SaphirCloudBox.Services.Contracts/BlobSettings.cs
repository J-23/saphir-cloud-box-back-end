using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts
{
    public class BlobSettings
    {
        public string ConnectionString { get; set; }

        public string ContainerName { get; set; }
    }
}
