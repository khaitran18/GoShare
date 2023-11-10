using Application.Common.Dtos;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Mappers
{
    public class DriverdocumentProfile : Profile
    {
        public DriverdocumentProfile()
        {
            CreateMap<Driverdocument, DriverDocumentDto>().ForMember(dest=>dest.Type,opt=>opt.MapFrom(src => (DocumentTypeEnumerations)src.Type));
            CreateMap<DriverDocumentDto, Driverdocument>().ForMember(dest=>dest.Type,opt=>opt.MapFrom(src => (short)src.Type));

        }
    }
}
