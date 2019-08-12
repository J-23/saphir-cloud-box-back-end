using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class AddFolderDto
    {
        public int ParentId { get; set; }

        public string Name { get; set; }
    }
}
