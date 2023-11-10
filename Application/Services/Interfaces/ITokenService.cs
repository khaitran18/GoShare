using Application.Common.Dtos;
using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ITokenService
    {
        public string GenerateJWTToken(Guid? id, string? phone, string? name, UserRoleEnumerations role);

        public string GenerateRefreshToken();

        /// <summary>
        /// using to extract claim from token from header.Authorization
        /// example use: ValidateToken(tokenString)?.FindFirst("ClaimName")?.Value
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public ClaimsPrincipal? ValidateToken(string jwtToken);
        public DateTime CreateRefreshTokenExpiryTime();
        public Guid GetGuid(string jwtToken);
    }
}
