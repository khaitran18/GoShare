﻿using Application.Common.Dtos;
using Application.Services.Interfaces;

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
            var serviceProvider = context.RequestServices;
            var claims = serviceProvider.GetService<UserClaims>();
            if (token is not null)
            {
                claims = _tokenService.CreateUserClaimsInstance(token);
            }
            await next(context);
        }
    }
}
