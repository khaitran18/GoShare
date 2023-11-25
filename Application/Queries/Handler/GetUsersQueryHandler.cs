using Application.Common.Dtos;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Handler
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<AdminUserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<AdminUserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            
            var response = new PaginatedResult<AdminUserResponse>();
            (List<User> list, int total) = await _unitOfWork.UserRepository.GetUsersAsync(request.Page, request.PageSize, request.SortBy);

            var responseList = _mapper.Map<List<AdminUserResponse>>(list);

            var paginatedResult = new PaginatedResult<AdminUserResponse>(
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
