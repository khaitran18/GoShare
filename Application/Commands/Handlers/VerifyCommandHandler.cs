using Application.Common.Utilities;
using Application.Services.Interfaces;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Exceptions;

namespace Application.Commands.Handlers
{
    public class VerifyCommandHandler : IRequestHandler<VerifyCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITwilioVerification _verificationService;

        public VerifyCommandHandler(IUnitOfWork unitOfWork, ITwilioVerification verificationService)
        {
            _unitOfWork = unitOfWork;
            _verificationService = verificationService;
        }

        public async Task<string> Handle(VerifyCommand request, CancellationToken cancellationToken)
        {
            DateTime OtpExpiryTime = await _unitOfWork.UserRepository.GetUserOtpExpiryTimeByPhone(request.Phone);
            if (OtpExpiryTime.CompareTo(DateTime.Now) < 0)
            {
                throw new UnauthorizedAccessException("Otp is expired");
            }
            else if (await _verificationService.CheckVerificationAsync(request.Phone, request.Otp))
            {
                var u = await _unitOfWork.UserRepository.GetUserByPhone(request.Phone);
                string PasscodeResetToken = OtpUtils.Generate();
                u!.Isverify = true;
                u!.PasscodeResetToken = PasswordHasher.Hash(PasscodeResetToken);
                u!.PasscodeResetTokenExpiryTime = DateTime.Now.AddMinutes(60);
                await _unitOfWork.UserRepository.UpdateAsync(u!);
                await _unitOfWork.Save();
                return PasscodeResetToken;
            }
            else throw new TwilioException("Otp is incorrect or expired");

            //else
            //{
            //    string? Otp = await _unitOfWork.UserRepository.GetUserOtpByPhone(request.Phone);
            //    if (PasswordHasher.Validate(Otp!, request.Otp)&&(Otp!=null))
            //    {
            //        var u = await _unitOfWork.UserRepository.GetUserByPhone(request.Phone);
            //        string PasscodeResetToken = OtpUtils.Generate();
            //        u!.Isverify = true;
            //        u!.PasscodeResetToken = PasswordHasher.Hash(PasscodeResetToken);
            //        u!.PasscodeResetTokenExpiryTime = DateTime.Now.AddMinutes(60);
            //        await _unitOfWork.UserRepository.UpdateAsync(u!);
            //        return PasscodeResetToken;
            //    }
            //    else throw new UnauthorizedAccessException("Incorrect Otp");
            //}

        }
    }
}
