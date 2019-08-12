using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class FileDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
