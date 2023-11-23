using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using AutoMapper;
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
    public class CreateDependentCommandHandler : IRequestHandler<CreateDependentCommand, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _claims;
        private readonly IMapper _mapper;
        private readonly ITwilioVerification _verificationService;

        public CreateDependentCommandHandler(IUnitOfWork unitOfWork, UserClaims claims, IMapper mapper, ITwilioVerification verificationService)
        {
            _unitOfWork = unitOfWork;
            _claims = claims;
            _mapper = mapper;
            _verificationService = verificationService;
        }

        public async Task<UserDto> Handle(CreateDependentCommand request, CancellationToken cancellationToken)
        {
            UserDto response = new UserDto();
            if (await _unitOfWork.UserRepository.IsDependent((Guid)_claims.id!)) 
                throw new ForbiddenAccessException("Dependent cannot create another dependent");
            else
            {
                if (await _unitOfWork.UserRepository.PhoneExist(request.Phone))
                {
                    throw new BadRequestException("Phone number existed");
                }
                else if (_verificationService.StartVerificationAsync(request.Phone, "sms").Result.Status.Equals("pending"))
                {
                    User user = new User();
                    user.Id = Guid.NewGuid();
                    user.Phone = request.Phone;
                    user.Name = request.Name;
                    user.Gender = request.Gender;
                    user.Birth = request.Birth;
                    user.CreateTime = DateTimeUtilities.GetDateTimeVnNow();
                    user.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    user.OtpExpiryTime = await _verificationService.GenerateOtpExpiryTime();
                    user.Isverify = false;
                    user.Isdriver = false;
                    user.GuardianId = (Guid)_claims.id!;
                    user.Status = Domain.Enumerations.UserStatus.INACTIVE;
                    await _unitOfWork.UserRepository.AddAsync(user);
                    await _unitOfWork.Save();
                    response = _mapper.Map<UserDto>(user);
                }
                else throw new TwilioException("Error in sending OTP, please try again later");
            }
            return response;
        }
    }
}
