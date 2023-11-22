using Application.Common.Dtos;
using AutoMapper;
using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Mappers
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            CreateMap<LocationDto, Location>()
                .ForMember(dest => dest.Longtitude, opt => opt.MapFrom(src => src.Longitude))
                .ReverseMap();
        }
    }
}
