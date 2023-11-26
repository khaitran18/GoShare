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
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();

            CreateMap<User, AdminUserResponse>()
                .ForMember(des=>des.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ReverseMap();

            CreateMap<User, AdminDriverResponse>()
                .ForMember(des => des.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(des => des.VerifyTo, opt => opt.MapFrom(src => src.Car!.VerifiedTo))
                .ReverseMap();
        }
    }
}
