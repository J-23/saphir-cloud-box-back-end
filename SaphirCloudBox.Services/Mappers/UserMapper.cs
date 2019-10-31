using Anthill.Common.Services;
using AutoMapper;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaphirCloudBox.Services.Mappers
{
    public class UserMapper : AbstractMapper<User, UserDto>, IUserMapper
    {
        protected override IMapper Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Client, ClientDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                    .ForMember(x => x.CreateDate, y => y.MapFrom(z => z.CreateDate))
                    .ForMember(x => x.UpdateDate, y => y.MapFrom(z => z.UpdateDate));

                cfg.CreateMap<Department, DepartmentDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                    .ForMember(x => x.CreateDate, y => y.MapFrom(z => z.CreateDate))
                    .ForMember(x => x.UpdateDate, y => y.MapFrom(z => z.UpdateDate))
                    .ForMember(x => x.Client, y => y.MapFrom(z => z.Client));

                cfg.CreateMap<User, UserDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.UserName, y => y.MapFrom(z => z.UserName))
                    .ForMember(x => x.Email, y => y.MapFrom(z => z.Email))
                    .ForMember(x => x.Email, y => y.MapFrom(z => z.Email))
                    .ForMember(x => x.Client, y => y.MapFrom(z => z.Client))
                    .ForMember(x => x.Department, y => y.MapFrom(z => z.Department))
                    .ForMember(x => x.CreateDate, y => y.MapFrom(z => z.CreateDate))
                    .ForMember(x => x.UpdateDate, y => y.MapFrom(z => z.UpdateDate))
                    .ForMember(x => x.GroupIds, y => y.MapFrom(z => z.UserInGroups.Select(s => s.GroupId).ToList()));
            });


            return config.CreateMapper();
        }
    }
}
