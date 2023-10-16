using Application.Common.Utilities;
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
    public class VerifyCommandHandler : IRequestHandler<VerifyCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VerifyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(VerifyCommand request, CancellationToken cancellationToken)
        {
            DateTime OtpExpiryTime = await _unitOfWork.UserRepository.GetUserOtpExpiryTimeByPhone(request.Phone);
            if (OtpExpiryTime.CompareTo(DateTime.Now)<0)
            {
                throw new UnauthorizedAccessException("Otp is expired");
            }
            else
            {
                string? Otp = await _unitOfWork.UserRepository.GetUserOtpByPhone(request.Phone);
                if (PasswordHasher.Validate(Otp!, request.Otp)&&(Otp!=null))
                {
                    var u = await _unitOfWork.UserRepository.GetUserByPhone(request.Phone);
                    string PasscodeResetToken = OtpUtils.Generate();
                    u!.Isverify = true;
                    u!.PasscodeResetToken = PasswordHasher.Hash(PasscodeResetToken);
                    u!.PasscodeResetTokenExpiryTime = DateTime.Now.AddMinutes(60);
                    await _unitOfWork.UserRepository.UpdateAsync(u!);
                    return PasscodeResetToken;
                }
                else throw new UnauthorizedAccessException("Incorrect Otp");
            }
        }
    }
}
