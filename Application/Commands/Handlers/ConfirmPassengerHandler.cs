using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Service;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class ConfirmPassengerHandler : IRequestHandler<ConfirmPassengerCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public ConfirmPassengerHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<bool> Handle(ConfirmPassengerCommand request, CancellationToken cancellationToken)
        {
            var trip = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);

            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), request.TripId);
            }

            if (request.Accept)
            {
                ClaimsPrincipal? claims = _tokenService.ValidateToken(request.Token ?? "");
                if (claims != null)
                {
                    Guid.TryParse(claims.FindFirst("jti")?.Value, out Guid driverId);
                    trip.DriverId = driverId;
                    trip.Status = TripStatus.GOING_TO_PICKUP;
                    trip.UpdatedTime = DateTime.Now;

                    KeyValueStore.Instance.Set($"TripConfirmationTask_{trip.Id}", "true");
                }
            }
            else
            {
                KeyValueStore.Instance.Set($"TripConfirmationTask_{trip.Id}", "false");
            }

            await _unitOfWork.TripRepository.UpdateAsync(trip);

            return true;
        }
    }
}
