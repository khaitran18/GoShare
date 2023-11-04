using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
namespace Application.Commands.Handlers
{
    public class AuthCommandHandler : IRequestHandler<AuthCommand, TokenResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<TokenResponse> Handle(AuthCommand request, CancellationToken cancellationToken)
        {
            TokenResponse response = new TokenResponse();
            var user = await _unitOfWork.UserRepository.GetUserByPhone(request.Phone);
            if (user == null)
            {
                throw new NotFoundException("Phone number not found");
            }
            else if (!user.Isverify)
            {
                throw new UnauthorizedAccessException("User is not verified");
            }
            else
            {
                if (!PasswordHasher.Validate(user.Passcode!, request.Passcode))
                {
                    throw new UnauthorizedAccessException("Wrong passcode");
                }
                else
                {
                    UserRoleEnumerations role = UserRoleEnumerations.User;
                    if (user.Isdriver) role = UserRoleEnumerations.Driver;
                    response.AccessToken = _tokenService.GenerateJWTToken(user.Id,user.Phone,user.Name,role);
                    response.RefreshToken = _tokenService.GenerateRefreshToken();
                    user.RefreshToken = response.RefreshToken;
                    user.RefreshTokenExpiryTime = _tokenService.CreateRefreshTokenExpiryTime();
                    await _unitOfWork.UserRepository.UpdateAsync(user);
                    await _unitOfWork.Save();
                }
            }
            return response;
        }
    }
}
