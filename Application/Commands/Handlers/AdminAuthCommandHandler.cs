using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Configuration;
using Application.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class AdminAuthCommandHandler : IRequestHandler<AdminAuthCommand, TokenResponse>
    {
        private readonly Admin _admin;
        private readonly ITokenService _tokenService;

        public AdminAuthCommandHandler(Admin admin, ITokenService tokenService)
        {
            _admin = admin;
            _tokenService = tokenService;
        }

        public async Task<TokenResponse> Handle(AdminAuthCommand request, CancellationToken cancellationToken)
        {
            TokenResponse response = new TokenResponse();
            if ((request.Username.Equals(_admin.Username)) && (request.Password.Equals(_admin.Password)))
            {
                response.AccessToken = _tokenService.GenerateJWTToken(new Guid("b18d7bd7-2589-4094-8814-9dde9dfb7178"), "0919651361", "Admin");
                response.RefreshToken = _tokenService.GenerateRefreshToken();
            }
            else throw new BadRequestException("Wrong username or password");
            return await Task.FromResult(response);
        }
    }
}
