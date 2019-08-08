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
    public class ClientMapper : AbstractMapper<Client, ClientDto>, IClientMapper
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
            });


            return config.CreateMapper();
        }
    }
}
