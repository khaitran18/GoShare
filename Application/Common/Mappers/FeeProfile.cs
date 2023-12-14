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
    public class FeeProfile : Profile
    {
        public FeeProfile()
        {
            Dictionary<Guid, int> carTypeMapping = new Dictionary<Guid, int>
        {
            { Guid.Parse("b3e29169-ac9b-437b-9715-ffe515e3724c"), 2 }, // 2 seats
            { Guid.Parse("60f79bc8-2853-492a-9d2e-cf2994102406"), 4 }, // 4 seats
            { Guid.Parse("ff7a9442-c9cf-4a06-bb3a-c40d49916480"), 7 }, // 6 seats
            { Guid.Parse("d8f6c348-b130-4b94-8ee0-c1fd4865ed5d"), 9 }  // 8 seats
        };
            CreateMap<Fee, FeeDto>()
                .ForMember(dest => dest.CarType, opt => opt.MapFrom(src => carTypeMapping[src.CarType]))
                .ReverseMap();
            CreateMap<Feepolicy, FeepolicyDto>().ReverseMap();
        }
    }
}
