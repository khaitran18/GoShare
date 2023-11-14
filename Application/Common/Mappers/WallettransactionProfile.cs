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
    public class WallettransactionProfile : Profile
    {
        public WallettransactionProfile()
        {
            CreateMap<Wallettransaction, VnpayCallbackResponse>()
                .ForMember(dest => dest.vnp_Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.vnp_TxnRef, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();
                
        }
    }
}
