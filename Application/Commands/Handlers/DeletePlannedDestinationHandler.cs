using Application.Common.Dtos;
using Application.Common.Exceptions;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class DeletePlannedDestinationHandler : IRequestHandler<DeletePlannedDestinationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _userClaims;

        public DeletePlannedDestinationHandler(IUnitOfWork unitOfWork, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _userClaims = userClaims;
        }

        public async Task<bool> Handle(DeletePlannedDestinationCommand request, CancellationToken cancellationToken)
        {
            Guid userId = (Guid)_userClaims.id!;
            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());

            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            var location = await _unitOfWork.LocationRepository.GetByIdAsync(request.Id);

            if (location == null)
            {
                throw new NotFoundException(nameof(Location), request.Id);
            }

            if (location.UserId != userId/* && (location.User.Guardian == null || location.User.GuardianId != userId)*/)
            {
                throw new BadRequestException("The user does not have permission to delete this location.");
            }

            // Check if the location is of type LocationType.PLANNED_DESTINATION
            if (location.Type != LocationType.PLANNED_DESTINATION)
            {
                throw new BadRequestException("The location is not a planned destination.");
            }

            await _unitOfWork.LocationRepository.DeleteAsync(location);
            await _unitOfWork.Save();

            return true;
        }
    }
}
