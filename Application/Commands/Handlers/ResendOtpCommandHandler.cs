using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services;
using Application.Services.Interfaces;
using Domain.Interfaces;
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
        private readonly ITwilioVerification _verificationService;
        public ResendOtpCommandHandler(IUnitOfWork unitOfWork, ITwilioVerification verificationService)
        {
            _unitOfWork = unitOfWork;
            _verificationService = verificationService;
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
                //string otp = OtpUtils.Generate();
                //await _SpeedSMSAPI.sendSMS(request.phone, "Ma OTP GoShare cua ban la: " + otp, 5);

                if (_verificationService.StartVerificationAsync(request.phone, "sms").IsCompletedSuccessfully)
                {
                    if (user.OtpExpiryTime!.Value.CompareTo(DateTime.Now) < 0)
                    {
                        user.OtpExpiryTime = await _verificationService.GenerateOtpExpiryTime();
                    }
                }
                //user.Otp = PasswordHasher.Hash(otp);
                //user.OtpExpiryTime = DateTime.Now.AddMinutes(10);

                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.Save();
            }
            return Task.CompletedTask;
        }
    }
}
