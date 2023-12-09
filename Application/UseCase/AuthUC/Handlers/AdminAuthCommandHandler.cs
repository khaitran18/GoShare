using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Configuration;
using Application.Services.Interfaces;
using Application.UseCase.AuthUC.Commands;
using Domain.Enumerations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuthUC.Handlers
{
    public class AdminAuthCommandHandler : IRequestHandler<AdminAuthCommand, AuthResponse>
    {
        private readonly Admin _admin;
        private readonly ITokenService _tokenService;

        public AdminAuthCommandHandler(Admin admin, ITokenService tokenService)
        {
            _admin = admin;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> Handle(AdminAuthCommand request, CancellationToken cancellationToken)
        {
            AuthResponse response = new AuthResponse();
            if (request.Username.Equals(_admin.Username) && request.Password.Equals(_admin.Password))
            {
                response.AccessToken = _tokenService.GenerateJWTToken(null, null, null, role: UserRoleEnumerations.Admin);
                response.RefreshToken = _tokenService.GenerateRefreshToken();
                //KeyValueStore.Instance.Set("Admin_RefreshToken", response.RefreshToken);
                //KeyValueStore.Instance.Set("Admin_RefreshToken_Expiry", _tokenService.CreateRefreshTokenExpiryTime());
            }
            else throw new BadRequestException("Wrong username or password");
            return await Task.FromResult(response);
        }
    }
}
