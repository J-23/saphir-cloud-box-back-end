using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class AddDepartmentDto
    {
        public string Name { get; set; }

        public int ClientId { get; set; }
    }
}
