using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.UseCase.UserUC.Queries;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.UserUC.Handlers
{
    public class GetDriverRegisterCodeQueryHandler : IRequestHandler<GetDriverRegisterCodeQuery, string?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _claims;

        public GetDriverRegisterCodeQueryHandler(IUnitOfWork unitOfWork, UserClaims claims)
        {
            _unitOfWork = unitOfWork;
            _claims = claims;
        }

        public async Task<string?> Handle(GetDriverRegisterCodeQuery request, CancellationToken cancellationToken)
        {
            string? response = null;
            Guid id = (Guid)_claims.id!;
            var u = await _unitOfWork.UserRepository.GetUserById(id.ToString());
            if (u is not null)
            {
                if (!u.Isdriver)
                {
                    u.Otp = PasswordHasher.Hash(id.ToString());
                    response = u.Otp;
                }
                else throw new ForbiddenAccessException("Driver is not allowed to use this function");
            }
            else throw new NotFoundException("User is not found");
            await _unitOfWork.Save();
            return response;
        }
    }
}
