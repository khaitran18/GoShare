using Application.Common.Dtos;
using Application.UseCase.ReportUC.Queries;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.ReportUC.Handlers
{
    public class GetReportsHandler : IRequestHandler<GetReportsQuery, PaginatedResult<ReportDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReportsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<ReportDto>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
        {
            var (reports, totalCount) = await _unitOfWork.ReportRepository.GetReports(
                request.Status,
                request.Page,
                request.PageSize
            );

            var reportDtos = _mapper.Map<List<ReportDto>>(reports);

            return new PaginatedResult<ReportDto>(
                reportDtos,
                totalCount,
                request.Page,
                request.PageSize
            );
        }
    }
}
