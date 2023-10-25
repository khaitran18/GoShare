using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services;
using Application.Services.Interfaces;
using Domain.DataModels;
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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Task>
    {
        private readonly ISpeedSMSAPI _SpeedSMSAPI;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(ISpeedSMSAPI speedSMSAPI, IUnitOfWork unitOfWork)
        {
            _SpeedSMSAPI = speedSMSAPI;
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
                string otp = OtpUtils.Generate();
                //await Task.FromResult(_SpeedSMSAPI.sendSMS(request.Phone, "Ma OTP GoShare cua ban la: " + otp, 5));
                User user = new User();
                user.Id = Guid.NewGuid();
                user.Phone = request.Phone;
                user.Name = request.Name;
                user.Gender = request.Gender;
                user.Birth = request.Birth;
                user.Otp = PasswordHasher.Hash(otp);
                user.OtpExpiryTime = DateTime.Now.AddMinutes(10);
                user.CreateTime = DateTime.Now;
                user.UpdatedTime = DateTime.Now;
                user.Isverify = false;
                user.Isdriver = false;
                user.Status = Domain.Enumerations.UserStatus.INACTIVE;
                await _unitOfWork.UserRepository.AddAsync(user);
                return Task.CompletedTask;
            }
        }
    }
}
