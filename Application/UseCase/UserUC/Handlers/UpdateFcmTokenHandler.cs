using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Application.UseCase.UserUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.UserUC.Handlers
{
    public class UpdateFcmTokenHandler : IRequestHandler<UpdateFcmTokenCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _userClaims;
        private readonly IMapper _mapper;

        public UpdateFcmTokenHandler(IUnitOfWork unitOfWork, UserClaims userClaims, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userClaims = userClaims;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(UpdateFcmTokenCommand request, CancellationToken cancellationToken)
        {
            var userDto = new UserDto();
            Guid userId = (Guid)_userClaims.id!;
            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());

            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            user.DeviceToken = request.FcmToken;
            user.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.Save();

            userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }
    }
}
