﻿using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Domain.Enumerations;
using System.Security.Claims;

namespace Api_Mobile.Middlewares
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
                    userClaims!.id = _tokenService.GetGuid(token);
                    userClaims.name = principal.FindFirst("name")?.Value.ToString();
                    userClaims.phone = principal.FindFirst("phone")?.Value.ToString();
                    userClaims.Role = 
                        principal.IsInRole(UserRoleEnumerations.User.ToString()) ? UserRoleEnumerations.User : principal.IsInRole(UserRoleEnumerations.Driver.ToString()) ? UserRoleEnumerations.Driver :
                        principal.IsInRole(UserRoleEnumerations.Dependent.ToString()) ? UserRoleEnumerations.Dependent
                         : UserRoleEnumerations.Admin;
                    userClaims.UserIp = UserUltilities.GetIpAddress(context);
                    await next(context);
                }
            }
            else await next(context);
        }
    }
}
