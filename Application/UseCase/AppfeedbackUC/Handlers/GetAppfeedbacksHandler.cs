using Application.Common;
using Application.Common.Dtos;
using Application.UseCase.AppfeedbackUC.Queries;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AppfeedbackUC.Handlers
{
    public class GetAppfeedbacksHandler : IRequestHandler<GetAppfeedbacksQuery, PaginatedResult<AppfeedbackDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAppfeedbacksHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<AppfeedbackDto>> Handle(GetAppfeedbacksQuery request, CancellationToken cancellationToken)
        {
            var response = new PaginatedResult<AppfeedbackDto>();

            var (appfeedbacks, totalCount) = await _unitOfWork.AppfeedbackRepository.GetAppfeedbacks(
                request.SortBy,
                request.Page,
                request.PageSize
            );

            var feedbackDtos = _mapper.Map<List<AppfeedbackDto>>(appfeedbacks);

            var paginatedResult = new PaginatedResult<AppfeedbackDto>(
                feedbackDtos,
                totalCount,
                request.Page,
                request.PageSize
            );

            response = paginatedResult;

            return response;
        }
    }
}
