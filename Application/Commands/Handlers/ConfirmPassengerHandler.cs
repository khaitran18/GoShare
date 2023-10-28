using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Service;
using Application.Services.Interfaces;
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

            if (trip.Status != TripStatus.PENDING)
            {
                throw new Exception("The trip is invalid.");
            }

            if (request.Accept)
            {
                ClaimsPrincipal? claims = _tokenService.ValidateToken(request.Token ?? "");
                Guid.TryParse(claims!.FindFirst("id")?.Value, out Guid driverId);

                var driver = await _unitOfWork.UserRepository.GetUserById(driverId.ToString());

                if (driver == null)
                {
                    throw new NotFoundException(nameof(User), driverId);
                }

                var car = await _unitOfWork.CarRepository.GetByUserId(driverId);

                if (car == null)
                {
                    throw new NotFoundException(nameof(Car), driverId);
                }

                if (car.TypeId != trip.CartypeId)
                {
                    throw new Exception("The driver's car type does not match the trip's car type.");
                }

                driver.Status = UserStatus.BUSY;
                driver.UpdatedTime = DateTime.Now;
                await _unitOfWork.UserRepository.UpdateAsync(driver);

                trip.DriverId = driverId;
                trip.Status = TripStatus.GOING_TO_PICKUP;
                trip.UpdatedTime = DateTime.Now;

                KeyValueStore.Instance.Set($"TripConfirmationTask_{trip.Id}", "true");

                await _unitOfWork.TripRepository.UpdateAsync(trip);

                return true;
            }
            else
            {
                KeyValueStore.Instance.Set($"TripConfirmationTask_{trip.Id}", "false");

                return false;
            }
        }
    }
}
