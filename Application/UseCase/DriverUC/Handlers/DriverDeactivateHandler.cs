using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.UseCase.DriverUC.Commands;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DriverUC.Handlers
{
    public class DriverDeactivateHandler : IRequestHandler<DriverDeactivateCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _userClaims;

        public DriverDeactivateHandler(IUnitOfWork unitOfWork, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _userClaims = userClaims;
        }

        public async Task<bool> Handle(DriverDeactivateCommand request, CancellationToken cancellationToken)
        {
            Guid userId = (Guid)_userClaims.id!;
            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());

            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            if (user.Status == UserStatus.BUSY)
            {
                throw new BadRequestException("You cannot do this function while you are in a trip.");
            }

            user.Status = UserStatus.INACTIVE;
            user.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.Save();

            return true;
        }
    }
}
