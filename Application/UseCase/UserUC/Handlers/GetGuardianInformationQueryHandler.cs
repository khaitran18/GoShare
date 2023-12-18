using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.UseCase.UserUC.Queries;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.UserUC.Handlers
{
    public class GetGuardianInformationQueryHandler : IRequestHandler<GetGuardianInformationQuery, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _claims;

        public GetGuardianInformationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims claims)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claims = claims;
        }

        public async Task<UserDto> Handle(GetGuardianInformationQuery request, CancellationToken cancellationToken)
        {
            var response = new UserDto();
            User? u = await _unitOfWork.UserRepository.GetUserById(_claims.id.ToString()!);
            if (u is null) throw new NotFoundException("User is not found");
            else
            {
                response = _mapper.Map<UserDto>(u.Guardian!);
            }
            return response;
        }
    }
}
