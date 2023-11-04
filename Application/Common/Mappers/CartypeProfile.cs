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
    public class CartypeProfile : Profile
    {
        public CartypeProfile()
        {
            CreateMap<CartypeDto, Cartype>().ReverseMap();
        }
    }
}
