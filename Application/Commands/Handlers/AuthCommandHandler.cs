using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
namespace Application.Commands.Handlers
{
    public class AuthCommandHandler : IRequestHandler<AuthCommand, AuthResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _userService = userService;
        }

        public async Task<AuthResponse> Handle(AuthCommand request, CancellationToken cancellationToken)
        {
            AuthResponse response = new AuthResponse();
            var user = await _unitOfWork.UserRepository.GetUserByPhone(request.Phone);
            if (user == null)
            {
                throw new NotFoundException("Phone number not found");
            }
            else if (!user.Isverify)
            {
                throw new UnauthorizedAccessException("User is not verified");
            }
            else if (await _unitOfWork.UserRepository.IsBanned(user.Id, out string? reason))
                throw new ForbiddenAccessException("User is banned: " + reason);
            else
            {
                if (!PasswordHasher.Validate(user.Passcode!, request.Passcode))
                {
                    throw new UnauthorizedAccessException("Wrong passcode");
                }
                else
                {
                    UserRoleEnumerations role = UserRoleEnumerations.User;
                    if (user.GuardianId is not null)
                        role = UserRoleEnumerations.Dependent;
                    //Generate tokens
                    response.AccessToken = _tokenService.GenerateJWTToken(user.Id,user.Phone,user.Name,role);
                    response.RefreshToken = _tokenService.GenerateRefreshToken();
                    //-------------------------------------------------------------------------------------
                    //Mapping
                    response.Id = user.Id;
                    response.Phone = user.Phone;
                    response.Name = user.Name;
                    //if user is a dependent response with dependent role, but jwt token keep role as user
                    //if (user.GuardianId is not null) 
                    //    role = UserRoleEnumerations.Dependent;
                    response.Role = role.ToString();
                    //------------------------------------------------------------------------------------

                    //Check if user is in any running trip
                    var trip = await _unitOfWork.TripRepository.GetCurrentTripByUserId(user.Id);
                    response.CurrentTrip = trip?.Id;

                    //Check if user's dependent is in any trip
                    response.DependentCurrentTrips = await _userService.GetCurrentDenpendentTrips(_unitOfWork, user.Id);
                    user.RefreshToken = response.RefreshToken;
                    user.RefreshTokenExpiryTime = _tokenService.CreateRefreshTokenExpiryTime();
                    await _unitOfWork.UserRepository.UpdateAsync(user);
                    await _unitOfWork.Save();
                }
            }
            return response;
        }
    }
}
