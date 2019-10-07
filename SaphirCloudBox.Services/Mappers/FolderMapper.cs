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
    public class FolderMapper : AbstractMapper<FileStorage, FolderDto>, IFolderMapper
    {
        protected override IMapper Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FileStorage, FolderDto>()
                    .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                    .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                    .ForMember(x => x.ParentId, y => y.MapFrom(z => z.ParentFileStorageId))
                    .ForMember(x => x.Children, y => y.Ignore());
            });


            return config.CreateMapper();
        }
    }
}
