using Anthill.Common.Services.Contracts;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Mappers
{
    public interface IDepartmentMapper: IMapper<Department, DepartmentDto>
    {
    }
}
