using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class AddFileDto
    {
        public int ParentId { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }
    }
}
