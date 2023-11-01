using Application.Common.Utilities;
using Application.Services.Interfaces;
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
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(IUnitOfWork unitOfWork,ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<string> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            string response = "";
            string? userId = null;
            ClaimsPrincipal claims = _tokenService.ValidateToken(request.AccessToken)!;
            string? RefreshToken;
            DateTime? RefreshTokenExpiryTime;
            if (claims.IsInRole(UserRoleEnumerations.Admin.ToString()))
            {
                RefreshToken = KeyValueStore.Instance.Get<string>("Admin_RefreshToken");
                RefreshTokenExpiryTime = KeyValueStore.Instance.Get<DateTime>("Admin_RefreshToken_Expiry");
            }
            else
            {
                userId = claims.FindFirst("id")!.Value;
                RefreshToken = await _unitOfWork.UserRepository.GetUserRefreshTokenByUserId(userId);
                RefreshTokenExpiryTime = await _unitOfWork.UserRepository.GetUserRefreshTokenExpiryTimeByUserId(userId);
            }
            if (RefreshTokenExpiryTime?.CompareTo(DateTime.Now) < 0) throw new UnauthorizedAccessException("Timeout, please login again");
            else
            {
                if (RefreshToken!.Equals(null)) throw new UnauthorizedAccessException("Token is null");
                else
                {
                    if (!RefreshToken.Equals(request.RefreshToken)) throw new Exception("Refresh token is currupted, please login again");
                    else
                    {
                        UserRoleEnumerations role = claims.IsInRole(UserRoleEnumerations.User.ToString()) ? UserRoleEnumerations.User : claims.IsInRole(UserRoleEnumerations.Driver.ToString()) ? UserRoleEnumerations.Driver : UserRoleEnumerations.Admin;
                        response = _tokenService.GenerateJWTToken(userId is not null ? new Guid(userId):null, claims.FindFirst("phone")?.Value, claims.FindFirst("name")?.Value, role);
                    }
                }

            }
            return response;
        }
    }
}
