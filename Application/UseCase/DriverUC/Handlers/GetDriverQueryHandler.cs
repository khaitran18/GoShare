using Application.Common.Dtos;
using Application.UseCase.DriverUC.Queries;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DriverUC.Handlers
{
    public class GetDriverQueryHandler : IRequestHandler<GetDriverQuery, PaginatedResult<AdminDriverResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDriverQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<AdminDriverResponse>> Handle(GetDriverQuery request, CancellationToken cancellationToken)
        {
            var response = new PaginatedResult<AdminDriverResponse>();

            (List<User> list, int total) = await _unitOfWork.UserRepository.GetDriverAsync(request.Page, request.PageSize, request.SortBy);

            var responseList = _mapper.Map<List<AdminDriverResponse>>(list);

            var paginatedResult = new PaginatedResult<AdminDriverResponse>(
                responseList,
                total,
                request.Page,
                request.PageSize
            );

            response = paginatedResult;

            return response;
        }
    }
}
