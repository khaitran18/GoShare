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
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
             CreateMap<Chat, ChatDto>()
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.Receiver))
                .ReverseMap();
        }
    }
}
