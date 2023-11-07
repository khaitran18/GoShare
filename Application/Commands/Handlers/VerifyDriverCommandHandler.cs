using Application.Services.Interfaces;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class VerifyDriverCommandHandler : IRequestHandler<VerifyDriverCommand, bool>
    {
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public VerifyDriverCommandHandler(ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(VerifyDriverCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.UserRepository.VerifyDriver(request.id);
            return _unitOfWork.Save().IsCompletedSuccessfully;
        }
    }
}
