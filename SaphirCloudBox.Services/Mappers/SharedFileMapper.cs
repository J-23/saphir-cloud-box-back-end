using Anthill.Common.Services;
using AutoMapper;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using SaphirCloudBox.Services.Contracts.Mappers;
using SaphirCloudBox.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaphirCloudBox.Services.Mappers
{
    public class SharedFileMapper : AbstractMapper<FileStoragePermission, SharedFileDto>, ISharedFileMapper
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

                cfg.CreateMap<Client, ClientDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.Name, y => y.MapFrom(z => z.Name));

                cfg.CreateMap<File, FileDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.Extension, y => y.MapFrom(z => z.Extension))
                    .ForMember(x => x.Size, y => y.MapFrom(z => z.Size))
                    .ForMember(x => x.SizeType, y => y.MapFrom(z => z.SizeType));

                cfg.CreateMap<FileStoragePermission, SharedFileDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.FileStorage.Id))
                    .ForMember(x => x.Name, y => y.MapFrom(z => z.FileStorage.Name))
                    .ForMember(x => x.IsDirectory, y => y.MapFrom(z => z.FileStorage.IsDirectory))
                    .ForMember(x => x.ParentStorageId, y => y.MapFrom(z => z.FileStorage.ParentFileStorageId))
                    .ForMember(x => x.PermissionType, y => y.MapFrom(z => z.Type))
                    .ForMember(x => x.Client, y => y.MapFrom(z => z.FileStorage.Client))
                    .ForMember(x => x.Owner, y => y.MapFrom(z => z.FileStorage.Owner))
                    .ForMember(x => x.CreateBy, y => y.MapFrom(z => z.FileStorage.CreateBy))
                    .ForMember(x => x.CreateDate, y => y.MapFrom(z => z.FileStorage.CreateDate))
                    .ForMember(x => x.UpdateBy, y => y.MapFrom(z => z.FileStorage.UpdateBy))
                    .ForMember(x => x.UpdateDate, y => y.MapFrom(z => z.FileStorage.UpdateDate))
                    .ForMember(x => x.File, y => y.MapFrom(z => z.FileStorage.Files.FirstOrDefault(f => f.IsActive)))
                    .ForMember(x => x.StorageType, y => y.MapFrom(z => StorageTypeUtil.GetStorageType(z.FileStorage.IsDirectory, z.FileStorage.Files)));
            });


            return config.CreateMapper();
        }
    }
}
