using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.UseCase.ReportUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.ReportUC.Handlers
{
    public class UpdateReportStatusHandler : IRequestHandler<UpdateReportStatusCommand, ReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateReportStatusHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReportDto> Handle(UpdateReportStatusCommand request, CancellationToken cancellationToken)
        {
            var report = await _unitOfWork.ReportRepository.GetByIdAsync(request.Id);
            if (report == null)
            {
                throw new NotFoundException(nameof(Report), request.Id);
            }

            report.Status = request.Status;
            report.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

            await _unitOfWork.ReportRepository.UpdateAsync(report);
            await _unitOfWork.Save();

            return _mapper.Map<ReportDto>(report);
        }
    }
}
