using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Service;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, Task>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISpeedSMSAPI _SpeedSMSAPI;
        public ResendOtpCommandHandler(IUnitOfWork unitOfWork, ISpeedSMSAPI speedSMSAPI)
        {
            _unitOfWork = unitOfWork;
            _SpeedSMSAPI = speedSMSAPI;
        }

        public async Task<Task> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetUserByPhone(request.phone);
            if (user == null)
            {
                throw new NotFoundException("Phone number not found");
            }
            else
            {
                string otp = OtpUtils.Generate();
                await _SpeedSMSAPI.sendSMS(request.phone, "Ma OTP GoShare cua ban la: " + otp, 5);
                user.Otp = PasswordHasher.Hash(otp);
                user.OtpExpiryTime = DateTime.Now.AddMinutes(10);
                await _unitOfWork.UserRepository.UpdateAsync(user);
            }
            return Task.CompletedTask;
        }
    }
}
