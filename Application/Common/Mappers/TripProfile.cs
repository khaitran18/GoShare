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
    public class TripProfile : Profile
    {
        public TripProfile()
        {
            CreateMap<TripDto, Trip>().ReverseMap();
            CreateMap<TripEndDto, Trip>().ReverseMap();
        }
    }
}
