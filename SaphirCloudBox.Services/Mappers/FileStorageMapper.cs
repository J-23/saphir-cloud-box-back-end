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
    public class FileStorageMapper : AbstractMapper<FileStorage, FileStorageDto.StorageDto>, IFileStorageMapper
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

                cfg.CreateMap<FileStorage, FileStorageDto.StorageDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                    .ForMember(x => x.CreateDate, y => y.MapFrom(z => z.CreateDate))
                    .ForMember(x => x.UpdateDate, y => y.MapFrom(z => z.UpdateDate))
                    .ForMember(x => x.CreateBy, y => y.MapFrom(z => z.CreateBy))
                    .ForMember(x => x.UpdateBy, y => y.MapFrom(z => z.UpdateBy))
                    .ForMember(x => x.IsDirectory, y => y.MapFrom(z => z.IsDirectory))
                    .ForMember(x => x.Owner, y => y.MapFrom(z => z.Owner));
            });


            return config.CreateMapper();
        }
    }
}
