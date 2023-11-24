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
            CreateMap<Wallettransaction, WalletTransactionDto>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ReverseMap();
        }
    }
}
