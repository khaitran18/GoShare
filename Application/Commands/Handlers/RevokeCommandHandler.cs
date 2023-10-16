using Application.Common.Exceptions;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class RevokeCommandHandler : IRequestHandler<RevokeCommand, Task>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RevokeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Task> Handle(RevokeCommand request, CancellationToken cancellationToken)
        {
            User? u = await _unitOfWork.UserRepository.GetUserById(request.id);
            if (u== null)
            {
                throw new NotFoundException("Id is not found");
            }
            else
            {
                u.RefreshToken = null;
                u.RefreshTokenExpiryTime = DateTime.Now;
                await _unitOfWork.UserRepository.UpdateAsync(u);
            }
            return Task.CompletedTask;
        }
    }
}
