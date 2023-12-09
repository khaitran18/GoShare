using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Application.UseCase.AuthUC.Commands;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuthUC.Handlers
{
    public class AuthDriverCommandHandler : IRequestHandler<AuthDriverCommand, AuthResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthDriverCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _userService = userService;
        }

        public async Task<AuthResponse> Handle(AuthDriverCommand request, CancellationToken cancellationToken)
        {
            AuthResponse response = new AuthResponse();
            var user = await _unitOfWork.UserRepository.GetUserByPhone(request.Phone);
            if (user == null)
            {
                throw new NotFoundException("Phone number not found");
            }
            else
            {
                if (!PasswordHasher.Validate(user.Passcode!, request.Passcode))
                    throw new UnauthorizedAccessException("Wrong passcode");
                else
                {
                    if (!user.Isdriver) throw new UnauthorizedAccessException("User is not driver");
                    else
                    {
                        UserRoleEnumerations role = UserRoleEnumerations.Driver;
                        response.AccessToken = _tokenService.GenerateJWTToken(user.Id, user.Phone, user.Name, role);
                        response.RefreshToken = _tokenService.GenerateRefreshToken();
                        response.Id = user.Id;
                        response.Phone = user.Phone;
                        response.Name = user.Name;
                        response.Role = role.ToString();
                        response.CurrentTrip = _unitOfWork.TripRepository.GetOngoingTripByPassengerId(user.Id).Result?.Id;
                        response.DependentCurrentTrips = await _userService.GetCurrentDenpendentTrips(_unitOfWork, user.Id);
                        user.RefreshToken = response.RefreshToken;
                        user.RefreshTokenExpiryTime = _tokenService.CreateRefreshTokenExpiryTime();
                        await _unitOfWork.UserRepository.UpdateAsync(user);
                        await _unitOfWork.Save();
                    }
                }
            }
            return response;
        }
    }
}
