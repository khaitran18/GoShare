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
    public class TestMapper : Profile
    {
        public TestMapper()
        {
            CreateMap<User, TestDto>()
                .ForMember(des=>des.user,otp=>otp.MapFrom(des=>des));
        }
    }
}
