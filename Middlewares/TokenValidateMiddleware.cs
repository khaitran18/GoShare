using Application.Common.Exceptions;
using Application.Service;
using Application.Services.Interfaces;
using System.Security.Claims;

namespace Api_Mobile.Middlewares
{
    public class TokenValidateMiddleware : IMiddleware
    {
        private readonly ITokenService _tokenService;

        public TokenValidateMiddleware(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string? token = context.Request.Headers.Authorization;
            if (token != null)
            {
                ClaimsPrincipal? principal = _tokenService.ValidateToken(token);
                if (principal != null)
                {
                    if (principal.FindFirst("id")!.Value.Equals(null)) throw new BadRequestException("Id value is not found");
                    if (principal.FindFirst("phone")!.Value.Equals(null)) throw new BadRequestException("Phone value is not found");
                    if (principal.FindFirst("name")!.Value.Equals(null)) throw new BadRequestException("Name value is not found");
                }
                else throw new BadRequestException("Token is currupted");
            }
            await next(context);
        }
    }
}
