using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.UseCase.AuthUC.Commands;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase.AuthUC.Handlers
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
            if (u == null)
            {
                throw new NotFoundException("Id is not found");
            }
            else
            {
                u.RefreshToken = null;
                u.RefreshTokenExpiryTime = DateTimeUtilities.GetDateTimeVnNow();
                await _unitOfWork.UserRepository.UpdateAsync(u);
                await _unitOfWork.Save();
            }
            return Task.CompletedTask;
        }
    }
}
