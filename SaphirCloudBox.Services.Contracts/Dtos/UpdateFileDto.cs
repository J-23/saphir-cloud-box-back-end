using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class UpdateFileDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public long? Size { get; set; }
    }
}
