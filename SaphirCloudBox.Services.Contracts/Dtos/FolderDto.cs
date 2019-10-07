using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class FolderDto
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public IEnumerable<FolderDto> Children { get; set; }

        public int NewFileCount { get; set; }
    }
}
