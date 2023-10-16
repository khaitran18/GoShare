﻿using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Service
{
    public interface ITokenService
    {
        public string GenerateJWTToken(Guid id, string phone, string? name);

        public string GenerateRefreshToken();

        /// <summary>
        /// using to extract claim from token from header.Authorization
        /// example use: ValidateToken(tokenString)?.FindFirst("ClaimName")?.Value
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public ClaimsPrincipal? ValidateToken(string jwtToken);
        public DateTime CreateRefreshTokenExpiryTime();
    }
    public class TokenService : ITokenService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _expiryMinutes;
        private readonly string _refreshTokenExpiryTime;

        public TokenService(string key, string expiryMinutes, string refreshTokenExpiryTime, string issuer, string audience)
        {
            _key = key;
            _expiryMinutes = expiryMinutes;
            _refreshTokenExpiryTime = refreshTokenExpiryTime;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateJWTToken(Guid id, string phone, string? name)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim("id", id.ToString()),
                new Claim("phone", phone),
                new Claim("name", name!),
 //               new Claim(ClaimTypes.Role,roles)
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_expiryMinutes)),
                signingCredentials: signingCredentials
           );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public DateTime CreateRefreshTokenExpiryTime()
        {
            return DateTime.Now.AddMinutes(Convert.ToDouble(_refreshTokenExpiryTime));
        }

        public ClaimsPrincipal? ValidateToken(string jwtToken)
        {
            if (jwtToken == "")
            {
                return null;
            }
            // token now include bearer, we dont need bearer
            var index = jwtToken.IndexOf(" ");
            if (index != -1)
            {
                jwtToken = jwtToken.Substring(index + 1);
            }

            IdentityModelEventSource.ShowPII = true;
            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();
            validationParameters.ValidateLifetime = true;
            validationParameters.ValidAudience = _audience.ToLower();
            validationParameters.ValidIssuer = _issuer.ToLower();
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);
            return principal;
        }
    }
}
