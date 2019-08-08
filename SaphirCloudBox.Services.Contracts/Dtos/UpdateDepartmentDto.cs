using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Dtos
{
    public class UpdateDepartmentDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ClientId { get; set; }
    }
}
