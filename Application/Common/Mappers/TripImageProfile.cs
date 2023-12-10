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
    public class TripImageProfile : Profile
    {
        public TripImageProfile()
        {
            CreateMap<TripImageDto, TripImage>().ReverseMap();
        }
    }
}
