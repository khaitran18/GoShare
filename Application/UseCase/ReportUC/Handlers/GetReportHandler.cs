using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.UseCase.ReportUC.Queries;
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
    public class GetReportHandler : IRequestHandler<GetReportQuery, ReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReportHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReportDto> Handle(GetReportQuery request, CancellationToken cancellationToken)
        {
            var report = await _unitOfWork.ReportRepository.GetByIdAsync(request.Id);
            if (report == null)
            {
                throw new NotFoundException(nameof(Report), request.Id);
            }

            return _mapper.Map<ReportDto>(report);
        }
    }
}
