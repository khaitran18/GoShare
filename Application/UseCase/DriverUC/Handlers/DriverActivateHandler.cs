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
    public class DriverActivateHandler : IRequestHandler<DriverActivateCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _userClaims;

        public DriverActivateHandler(IUnitOfWork unitOfWork, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _userClaims = userClaims;
        }

        public async Task<bool> Handle(DriverActivateCommand request, CancellationToken cancellationToken)
        {
            Guid userId = (Guid)_userClaims.id!;
            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());

            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            // Check if the user is suspended
            if (user.Status == UserStatus.SUSPENDED)
            {
                if (user.DisabledReason != null)
                {
                    throw new BadRequestException(user.DisabledReason);
                }
                else
                {
                    throw new BadRequestException("Tài khoản của bạn đã bị khóa tạm thời và không thể thực hiện hành động này. " +
                        "Vui lòng kiểm tra lại tài khoản của bạn.");
                }
            }

            if (user.Status == UserStatus.BUSY)
            {
                throw new BadRequestException("You cannot do this function while you are in a trip.");
            }

            user.Status = UserStatus.ACTIVE;
            user.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.Save();

            return true;
        }
    }
}
