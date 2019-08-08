using Anthill.Common.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Mappers
{
    public class RoleMapper : AbstractMapper<IdentityRole<int>, RoleDto>, IRoleMapper
    {
        protected override IMapper Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IdentityRole<int>, RoleDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.Name, y => y.MapFrom(z => z.Name));
            });


            return config.CreateMapper();
        }
    }
}
