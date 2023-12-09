using Application.Common.Dtos;
using Application.UseCase.UserUC.Queries;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.UserUC.Handlers
{
    public class GetDependentsHandler : IRequestHandler<GetDependentsQuery, PaginatedResult<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;

        public GetDependentsHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClaims = userClaims;
        }

        public async Task<PaginatedResult<UserDto>> Handle(GetDependentsQuery request, CancellationToken cancellationToken)
        {
            Guid id = (Guid)_userClaims.id!;
            var (dependents, totalCount) = await _unitOfWork.UserRepository.GetDependents(
                id,
                request.SortBy,
                request.Page,
                request.PageSize
            );

            var dependentDtos = _mapper.Map<List<UserDto>>(dependents);

            var paginatedResult = new PaginatedResult<UserDto>(
                dependentDtos,
                totalCount,
                request.Page,
                request.PageSize
            );

            return paginatedResult;
        }
    }
}
