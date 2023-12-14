using Application.Common.Dtos;
using Application.Common.Exceptions;
using Domain.Enumerations;
using Domain.Interfaces;

namespace Api_Mobile.Middlewares
{
    public class CheckUserVerificationMiddleware : IMiddleware
    {
        private readonly UserClaims _userClaims;
        private readonly IUnitOfWork _unitOfWork;

        public CheckUserVerificationMiddleware(UserClaims userClaims, IUnitOfWork unitOfWork)
        {
            _userClaims = userClaims;
            _unitOfWork = unitOfWork;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (_userClaims.id is not null)
            {
                Guid UserId = (Guid)_userClaims.id;
                if (!await _unitOfWork.UserRepository.IsVerified(UserId))
                    throw new ForbiddenAccessException("User is not verified");
                if (await _unitOfWork.UserRepository.IsBanned(UserId, out string? reason))
                    throw new ForbiddenAccessException("User is banned: " + reason);
                //if (_userClaims.Role.Equals(UserRoleEnumerations.Driver))
                //{
                //    if (!await _unitOfWork.CarRepository.IsValidByDate(UserId))
                //        throw new ForbiddenAccessException("User's car is invalid");
                //}
            }
            await next(context);
        }
    }
}
