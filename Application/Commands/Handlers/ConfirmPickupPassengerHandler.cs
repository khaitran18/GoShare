using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using AutoMapper;
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
    public class ConfirmPickupPassengerHandler : IRequestHandler<ConfirmPickupPassengerCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;
        private readonly UserClaims _userClaims;

        public ConfirmPickupPassengerHandler(IUnitOfWork unitOfWork, IMapper mapper, ISettingService settingService, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _settingService = settingService;
            _userClaims = userClaims;
        }

        public async Task<TripDto> Handle(ConfirmPickupPassengerCommand request, CancellationToken cancellationToken)
        {
            var tripDto = new TripDto();

            var trip = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);

            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), request.TripId);
            }

            if (trip.Status != TripStatus.GOING_TO_PICKUP)
            {
                throw new BadRequestException("The trip is invalid.");
            }

            Guid driverId = (Guid)_userClaims.id!;

            if (trip.DriverId != driverId)
            {
                throw new BadRequestException("The driver does not match for this trip.");
            }

            var driverLocation = await _unitOfWork.LocationRepository.GetByUserIdAndTypeAsync(driverId, LocationType.CURRENT_LOCATION);

            if (driverLocation == null)
            {
                throw new NotFoundException(nameof(Location), driverId);
            }

            driverLocation.Latitude = request.DriverLatitude;
            driverLocation.Longtitude = request.DriverLongitude;
            driverLocation.UpdatedTime = DateTime.Now;

            await _unitOfWork.LocationRepository.UpdateAsync(driverLocation);

            await _unitOfWork.Save();

            var startLocation = await _unitOfWork.LocationRepository.GetByIdAsync(trip.StartLocationId);

            if (startLocation == null)
            {
                throw new NotFoundException(nameof(Location), trip.StartLocationId);
            }

            var distance = MapsUtilities.GetDistance(driverLocation, startLocation);

            if (distance > _settingService.GetSetting("NEAR_DESTINATION_DISTANCE")) //km
            {
                throw new BadRequestException("The driver is not near the pickup location.");
            }

            trip.Status = TripStatus.GOING;
            trip.PickupTime = DateTime.Now;
            trip.UpdatedTime = DateTime.Now;

            await _unitOfWork.TripRepository.UpdateAsync(trip);

            await _unitOfWork.Save();

            tripDto = _mapper.Map<TripDto>(trip);

            return tripDto;
        }
    }
}
