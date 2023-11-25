using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Domain.Enumerations;
using System.Security.Claims;

namespace Api_Admin.Middlewares
{
    public class GetUserClaimsMiddleware : IMiddleware
    {
        private readonly ITokenService _tokenService;
        public GetUserClaimsMiddleware(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string token = context.Request.Headers.Authorization;
            if (token is not null)
            {
                var serviceProvider = context.RequestServices;
                using (var userClaims = serviceProvider.GetService<UserClaims>())
                {
                    ClaimsPrincipal principal = _tokenService.ValidateToken(token)!;
                    if (principal.IsInRole(UserRoleEnumerations.Admin.ToString())) 
                        userClaims!.Role = UserRoleEnumerations.Admin;
                    else throw new UnauthorizedAccessException();
                    userClaims.UserIp = UserUltilities.GetIpAddress(context);
                    await next(context);
                }
            }
            else await next(context);
        }
    }
}
