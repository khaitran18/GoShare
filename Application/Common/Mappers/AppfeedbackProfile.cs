using AutoMapper;
using Application.Common.Dtos;
using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Mappers
{
    public class AppfeedbackProfile : Profile
    {
        public AppfeedbackProfile()
        {
            CreateMap<AppfeedbackDto, Appfeedback>().ReverseMap();
        }
    }
}
