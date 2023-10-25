using Application.Service;
using Application.Services.Interfaces;
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
            string response;
            ClaimsPrincipal claims = _tokenService.ValidateToken(request.AccessToken)!;
            string userId = claims.FindFirst("id")!.Value;
            string? RefreshToken = await _unitOfWork.UserRepository.GetUserRefreshTokenByUserId(userId);
            DateTime? RefreshTokenExpiryTime = await _unitOfWork.UserRepository.GetUserRefreshTokenExpiryTimeByUserId(userId);
            if (RefreshTokenExpiryTime?.CompareTo(DateTime.Now) < 0) throw new UnauthorizedAccessException("Timeout, please login again");
            else
            {
                if (RefreshToken!.Equals(null)) throw new UnauthorizedAccessException("Token is null");
                else
                {
                    if (!RefreshToken.Equals(request.RefreshToken)) throw new Exception("Refresh token is currupted, please login again");
                    else
                    {
                        response = _tokenService.GenerateJWTToken(new Guid(userId), claims.FindFirst("phone")!.Value, claims.FindFirst("name")!.Value);
                    }
                }
                
            }
            return response;
        }
    }
}
