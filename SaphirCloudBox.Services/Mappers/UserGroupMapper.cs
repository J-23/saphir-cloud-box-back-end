using Anthill.Common.Services;
using AutoMapper;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Dtos.UserGroup;
using SaphirCloudBox.Services.Contracts.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaphirCloudBox.Services.Mappers
{
    public class UserGroupMapper : AbstractMapper<Group, UserGroupDto>, IUserGroupMapper
    {
        protected override IMapper Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>();

                cfg.CreateMap<Group, UserGroupDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                    .ForMember(x => x.Users, y => y.MapFrom(z => z.UsersInGroup.Select(s => s.User).ToList()));
            });


            return config.CreateMapper();
        }
    }
}
