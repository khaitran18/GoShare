using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.UseCase.SettingUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.SettingUC.Handlers
{
    public class UpdateSettingHandler : IRequestHandler<UpdateSettingCommand, SettingDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateSettingHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SettingDto> Handle(UpdateSettingCommand request, CancellationToken cancellationToken)
        {
            var setting = await _unitOfWork.SettingRepository.GetSettingById(request.Id);

            if (setting == null)
            {
                throw new NotFoundException(nameof(Setting), request.Id);
            }

            if (request.Value.HasValue)
            {
                setting.Value = request.Value.Value;
                await _unitOfWork.SettingRepository.UpdateAsync(setting);
            }

            await _unitOfWork.Save();

            var settingDto = _mapper.Map<SettingDto>(setting);

            return settingDto;
        }
    }
}
