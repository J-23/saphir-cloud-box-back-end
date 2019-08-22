using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class FileDto
    {
        public int Id { get; set; }

        public string Extension { get; set; }

        public int Size { get; set; }

        public string SizeType { get; set; }
    }
}
