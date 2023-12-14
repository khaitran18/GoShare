using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.UseCase.UserUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.UserUC.Handlers
{
    public class BanUserHandler : IRequestHandler<BanUserCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BanUserHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(BanUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(request.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException(nameof(User), request.UserId);
            }

            if (user.Status == UserStatus.BUSY)
            {
                throw new BadRequestException("Cannot ban a busy user");
            }

            user.Status = UserStatus.BANNED;
            user.DisabledReason = request.DisabledReason;

            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.Save();

            return _mapper.Map<UserDto>(user);
        }
    }
}
