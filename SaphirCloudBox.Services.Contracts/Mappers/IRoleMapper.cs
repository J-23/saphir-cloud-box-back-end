using Anthill.Common.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Contracts.Mappers
{
    public interface IRoleMapper: IMapper<Role, RoleDto>
    {
    }
}
