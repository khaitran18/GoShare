using Application.Common.Dtos;
using Application.UseCase.SettingUC.Queries;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.SettingUC.Handlers
{
    public class GetSettingsHandler : IRequestHandler<GetSettingsQuery, List<SettingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSettingsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<SettingDto>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
        {
            var settings = await _unitOfWork.SettingRepository.GetAllAsync();

            var settingDtos = _mapper.Map<List<SettingDto>>(settings);

            return settingDtos;
        }
    }
}
