using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services;
using Application.Services.Interfaces;
using Domain.DataModels;
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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Task>
    {
        private readonly ITwilioVerification _verificationService;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(ITwilioVerification verificationService, IUnitOfWork unitOfWork)
        {
            _verificationService = verificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Task> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.UserRepository.PhoneExist(request.Phone))
            {
                throw new BadRequestException("Phone number existed");
            }
            else
            {
                //string otp = OtpUtils.Generate();
                if (_verificationService.StartVerificationAsync(request.Phone, "sms").Result.Status.Equals("pending"))
                {
                    User user = new User();
                    user.Id = Guid.NewGuid();
                    user.Phone = request.Phone;
                    user.Name = request.Name;
                    user.Gender = request.Gender;
                    user.Birth = request.Birth;
                    //user.Otp = PasswordHasher.Hash(otp);
                    user.OtpExpiryTime = await _verificationService.GenerateOtpExpiryTime();
                    user.CreateTime = DateTime.Now;
                    user.UpdatedTime = DateTime.Now;
                    user.Isverify = false;
                    user.Isdriver = false;
                    user.Status = Domain.Enumerations.UserStatus.INACTIVE;
                    await _unitOfWork.UserRepository.AddAsync(user);
                    await _unitOfWork.Save();
                    return Task.CompletedTask;
                }
                else throw new TwilioException("Error in sending OTP, please try again later");
            }
        }
    }
}
