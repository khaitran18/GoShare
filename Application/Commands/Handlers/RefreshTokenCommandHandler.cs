using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.Services.Interfaces;
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

namespace Application.Commands.Handlers
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
            //ClaimsPrincipal claims = _tokenService.ValidateToken(request.AccessToken)!;
            IEnumerable < Claim > claims = _tokenService.GetTokenClaims(request.AccessToken)!;
            string? RefreshToken;
            DateTime? RefreshTokenExpiryTime;
            string roleClaims = claims.First(u => u.Type.Equals(ClaimTypes.Role)).Value;
            //if (claims.IsInRole(UserRoleEnumerations.Admin.ToString()))
            if (roleClaims.Equals(UserRoleEnumerations.Admin.ToString()))
            {
                RefreshToken = KeyValueStore.Instance.Get<string>("Admin_RefreshToken");
                RefreshTokenExpiryTime = KeyValueStore.Instance.Get<DateTime>("Admin_RefreshToken_Expiry");
            }
            else
            {
                userId = claims.FirstOrDefault(u => u.Type.Equals("id"))!.Value;
                RefreshToken = await _unitOfWork.UserRepository.GetUserRefreshTokenByUserId(userId);
                RefreshTokenExpiryTime = await _unitOfWork.UserRepository.GetUserRefreshTokenExpiryTimeByUserId(userId);
            }
            if (RefreshTokenExpiryTime?.CompareTo(DateTimeUtilities.GetDateTimeVnNow()) < 0) throw new UnauthorizedAccessException("Timeout, please login again");
            else
            {
                if (RefreshToken!.Equals(null)) throw new UnauthorizedAccessException("Token is null");
                else
                {
                    if (!RefreshToken.Equals(request.RefreshToken)) throw new Exception("Refresh token is currupted, please login again");
                    else
                    {

                        UserRoleEnumerations role = roleClaims.Equals(UserRoleEnumerations.User.ToString())|| roleClaims.Equals(UserRoleEnumerations.Dependent.ToString()) ? UserRoleEnumerations.User : roleClaims.Equals(UserRoleEnumerations.Driver.ToString()) ? UserRoleEnumerations.Driver : roleClaims.Equals(UserRoleEnumerations.Driver.ToString())?UserRoleEnumerations.Dependent:UserRoleEnumerations.Admin;
                        response.AccessToken = _tokenService.GenerateJWTToken(userId is not null ? new Guid(userId):null, claims.First(u=>u.Type.Equals("phone"))?.Value, claims.First(u=>u.Type.Equals("name"))?.Value, role);
                        response.Role = roleClaims;
                        if (await _unitOfWork.UserRepository.IsDependent(new Guid(userId!))) response.Role = UserRoleEnumerations.Dependent.ToString();
                        response.RefreshToken = request.RefreshToken;
                        response.Phone = claims.First(u => u.Type.Equals("phone"))?.Value;
                        response.Name = claims.First(u => u.Type.Equals("name"))?.Value;
                        response.Id = new Guid(userId!);
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
