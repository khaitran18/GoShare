using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Application.UseCase.AuthUC.Commands;
using AutoMapper;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuthUC.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _userService = userService;
        }

        public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            AuthResponse response = new AuthResponse();
            string? userId = null;
            ClaimsPrincipal claims = _tokenService.ValidateToken(request.AccessToken)!;
            string? RefreshToken;
            DateTime? RefreshTokenExpiryTime;
            userId = claims.FindFirst("id")!.Value;
            RefreshToken = await _unitOfWork.UserRepository.GetUserRefreshTokenByUserId(userId);
            RefreshTokenExpiryTime = await _unitOfWork.UserRepository.GetUserRefreshTokenExpiryTimeByUserId(userId);
            if (RefreshTokenExpiryTime?.CompareTo(DateTimeUtilities.GetDateTimeVnNow()) < 0) throw new UnauthorizedAccessException("Timeout, please login again");
            else
            {
                if (RefreshToken is null) throw new UnauthorizedAccessException("Token is null");
                else
                {
                    if (!RefreshToken.Equals(request.RefreshToken)) throw new Exception("Refresh token is currupted, please login again");
                    else
                    {
                        UserRoleEnumerations role =
                            claims.IsInRole(UserRoleEnumerations.User.ToString()) ? UserRoleEnumerations.User :
                            claims.IsInRole(UserRoleEnumerations.Driver.ToString()) ? UserRoleEnumerations.Driver : UserRoleEnumerations.Dependent;
                        response.Role = role.ToString();
                        response.RefreshToken = request.RefreshToken;
                        response.Phone = claims.FindFirst("phone")!.Value;
                        response.Name = claims.FindFirst("name")!.Value;
                        response.Id = new Guid(userId!);
                        response.AccessToken = _tokenService.GenerateJWTToken(response.Id, response.Phone, response.Name, role);
                        var trip = await _unitOfWork.TripRepository.GetCurrentTripByUserId((Guid)response.Id);
                        response.CurrentTrip = trip?.Id;
                        response.DependentCurrentTrips = await _userService.GetCurrentDenpendentTrips(_unitOfWork, new Guid(userId!));
                    }
                }
            }
            return response;
        }
    }
}
