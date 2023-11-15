using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Common.Utilities.Google;
using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class CreateTripHandler : IRequestHandler<CreateTripCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;
        private readonly UserClaims _userClaims;
        private readonly ILogger<BackgroundServices> _logger;

        public CreateTripHandler(IUnitOfWork unitOfWork, IMapper mapper, ISettingService settingService, UserClaims userClaims, ILogger<BackgroundServices> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _settingService = settingService;
            _userClaims = userClaims;
            _logger = logger;
        }

        public async Task<TripDto> Handle(CreateTripCommand request, CancellationToken cancellationToken)
        {
            var tripDto = new TripDto();

            Guid userId = (Guid)_userClaims.id!;

            var passenger = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (passenger == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            var now = DateTimeUtilities.GetDateTimeVnNow();
            var cancellationWindowMinutes = _settingService.GetSetting("TRIP_CANCELLATION_WINDOW");
            var cancellationLimit = _settingService.GetSetting("TRIP_CANCELLATION_LIMIT");
            var banDurationMinutes = _settingService.GetSetting("CANCELLATION_BAN_DURATION");

            var cancellationWindow = now.AddMinutes(-cancellationWindowMinutes);

            // Check if the user has cancelled too many trips recently
            if (passenger.LastTripCancellationTime >= cancellationWindow && passenger.CanceledTripCount >= cancellationLimit)
            {
                if (now < passenger.CancellationBanUntil)
                {
                    throw new BadRequestException($"You have exceeded the maximum number of cancellations allowed within {cancellationWindowMinutes} minutes. Please wait until {passenger.CancellationBanUntil} before creating a new trip.");
                }
                else
                {
                    // If the ban duration has passed, reset
                    passenger.CanceledTripCount = 0;
                    passenger.LastTripCancellationTime = null;
                    passenger.CancellationBanUntil = null;
                    passenger.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    await _unitOfWork.UserRepository.UpdateAsync(passenger);
                }
            }

            var currentLocation = await _unitOfWork.LocationRepository.GetByUserIdAndTypeAsync(userId, LocationType.CURRENT_LOCATION);
            if (currentLocation == null)
            {
                currentLocation = new Location
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Address = request.StartAddress,
                    Latitude = request.StartLatitude,
                    Longtitude = request.StartLongitude,
                    Type = LocationType.CURRENT_LOCATION,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.LocationRepository.AddAsync(currentLocation);
            }
            else
            {
                currentLocation.Address = request.StartAddress;
                currentLocation.Latitude = request.StartLatitude;
                currentLocation.Longtitude = request.StartLongitude;
                currentLocation.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

                await _unitOfWork.LocationRepository.UpdateAsync(currentLocation);
            }

            var pastOrigin = new Location
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Address = currentLocation.Address,
                Latitude = currentLocation.Latitude,
                Longtitude = currentLocation.Longtitude,
                Type = LocationType.PAST_ORIGIN,
                CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
            };

            await _unitOfWork.LocationRepository.AddAsync(pastOrigin);

            var destination = await _unitOfWork.LocationRepository.GetByUserIdAndLatLongAsync(userId, request.EndLatitude, request.EndLongitude);
            if (destination == null)
            {
                destination = new Location
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Address = request.EndAddress,
                    Latitude = request.EndLatitude,
                    Longtitude = request.EndLongitude,
                    Type = LocationType.PAST_DESTINATION,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.LocationRepository.AddAsync(destination);
            }

            var distance = await GoogleMapsApiUtilities.ComputeDistanceMatrixAsync(pastOrigin, destination);

            var totalPrice = await _unitOfWork.CartypeRepository.CalculatePriceForCarType(request.CartypeId, distance);

            if (request.PaymentMethod == PaymentMethod.WALLET)
            {
                Guid walletOwnerId = passenger.GuardianId ?? passenger.Id;

                var walletOwnerWallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(walletOwnerId);
                if (walletOwnerWallet == null)
                {
                    throw new NotFoundException(nameof(Wallet), walletOwnerId);
                }
                if (walletOwnerWallet.Balance < totalPrice)
                {
                    throw new BadRequestException("The wallet owner's wallet does not have enough balance.");
                }
            }

            var trip = new Trip
            {
                Id = Guid.NewGuid(),
                PassengerId = userId,
                StartLocationId = pastOrigin.Id,
                EndLocationId = destination.Id,
                StartTime = DateTimeUtilities.GetDateTimeVnNow(),
                CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                UpdatedTime = DateTimeUtilities.GetDateTimeVnNow(),
                Distance = distance,
                CartypeId = request.CartypeId,
                Price = totalPrice,
                Status = TripStatus.PENDING,
                PaymentMethod = request.PaymentMethod,
                BookerId = userId,
                Note = request.Note
            };

            await _unitOfWork.TripRepository.AddAsync(trip);

            await _unitOfWork.Save();

            tripDto = _mapper.Map<TripDto>(trip);

            // Background task
            //var cts = _serviceProvider.GetRequiredService<CancellationTokenSource>();
            string jobId = BackgroundJob.Enqueue<BackgroundServices>(s => s.FindDriver(trip.Id, request.CartypeId));
            //KeyValueStore.Instance.Set($"FindDriverTask_{trip.Id}", jobId);

            return tripDto;
        }
    }
}