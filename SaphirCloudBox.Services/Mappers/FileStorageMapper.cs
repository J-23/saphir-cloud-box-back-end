﻿using Anthill.Common.Services;
using AutoMapper;
using SaphirCloudBox.Enums;
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
    public class FileStorageMapper : AbstractMapper<FileStorage, FileStorageDto.StorageDto>, IFileStorageMapper
    {
        protected override IMapper Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>()
                    .ForMember(x => x.UserName, y => y.MapFrom(z => z.UserName))
                    .ForMember(x => x.Client, y => y.MapFrom(z => z.Client))
                    .ForMember(x => x.Department, y => y.Ignore())
                    .ForMember(x => x.Role, y => y.Ignore())
                    .ForMember(x => x.GroupIds, y => y.MapFrom(z => z.UserInGroups.Select(s => s.GroupId)));

                cfg.CreateMap<Client, ClientDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.Name, y => y.MapFrom(z => z.Name));

                cfg.CreateMap<File, FileDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.Extension, y => y.MapFrom(z => z.Extension))
                    .ForMember(x => x.Size, y => y.MapFrom(z => z.Size))
                    .ForMember(x => x.SizeType, y => y.MapFrom(z => z.SizeType));

                cfg.CreateMap<FileStoragePermission, FileStorageDto.StorageDto.PermissionDto>()
                    .ForMember(x => x.Recipient, y => y.MapFrom(x => x.Recipient))
                    .ForMember(x => x.Type, y => y.MapFrom(x => x.Type));

                cfg.CreateMap<FileStorage, FileStorageDto.StorageDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.ParentStorageId, y => y.MapFrom(z => z.ParentFileStorageId))
                    .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                    .ForMember(x => x.CreateDate, y => y.MapFrom(z => z.CreateDate))
                    .ForMember(x => x.UpdateDate, y => y.MapFrom(z => z.UpdateDate))
                    .ForMember(x => x.CreateBy, y => y.MapFrom(z => z.CreateBy))
                    .ForMember(x => x.UpdateBy, y => y.MapFrom(z => z.UpdateBy))
                    .ForMember(x => x.IsDirectory, y => y.MapFrom(z => z.IsDirectory))
                    .ForMember(x => x.Owner, y => y.MapFrom(z => z.Owner))
                    .ForMember(x => x.Client, y => y.MapFrom(z => z.Client))
                    .ForMember(x => x.StorageType, y => y.MapFrom(z => StorageTypeUtil.GetStorageType(z.IsDirectory, z.Files)))
                    .ForMember(x => x.File, y => y.MapFrom(z => z.Files.FirstOrDefault(f => f.IsActive)))
                    .ForMember(x => x.Permissions, y => y.MapFrom(z => z.Permissions.Where(x => !x.EndDate.HasValue).ToList()))
                    .ForMember(x => x.PermissionInfo, y => y.Ignore());
            });


            return config.CreateMapper();
        }
    }
}
