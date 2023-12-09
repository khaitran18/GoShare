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
    public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;

        public GetUserHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClaims = userClaims;
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            Guid userId = request.UserId ?? (Guid)_userClaims.id!;

            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            var userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }
    }

}
