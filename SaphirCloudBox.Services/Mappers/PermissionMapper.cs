using Anthill.Common.Services;
using AutoMapper;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Mappers
{
    public class PermissionMapper : AbstractMapper<FileStoragePermission, FileStorageDto.StorageDto.PermissionDto>, IPermissionMapper
    {
        protected override IMapper Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>()
                    .ForMember(x => x.UserName, y => y.MapFrom(z => z.UserName))
                    .ForMember(x => x.Client, y => y.Ignore())
                    .ForMember(x => x.Department, y => y.Ignore())
                    .ForMember(x => x.Role, y => y.Ignore());

                cfg.CreateMap<FileStoragePermission, FileStorageDto.StorageDto.PermissionDto>()
                    .ForMember(x => x.Recipient, y => y.MapFrom(x => x.Recipient))
                    .ForMember(x => x.Type, y => y.MapFrom(x => x.Type));
            });


            return config.CreateMapper();
        }
    }
}
