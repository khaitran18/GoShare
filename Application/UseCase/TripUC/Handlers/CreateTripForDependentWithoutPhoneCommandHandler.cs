
using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Common.Utilities.Google;
using Application.Services;
using Application.Services.Interfaces;
using Application.UseCase.TripUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.TripUC.Handlers
{
    public class CreateTripForDependentWithoutPhoneCommandHandler : IRequestHandler<CreateTripForDependentWithoutPhoneCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;
        private readonly UserClaims _userClaims;
        private readonly ILogger<BackgroundServices> _logger;

        public CreateTripForDependentWithoutPhoneCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ISettingService settingService, UserClaims userClaims, ILogger<BackgroundServices> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _settingService = settingService;
            _userClaims = userClaims;
            _logger = logger;
        }

        public async Task<TripDto> Handle(CreateTripForDependentWithoutPhoneCommand request, CancellationToken cancellationToken)
        {
            var tripDto = new TripDto();

            Guid userId = (Guid)_userClaims.id!;

            var booker = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (booker == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            // Prevent dependents from creating trip
            if (booker.GuardianId != null)
            {
                throw new BadRequestException("Dependents are not allowed to create trips. Please contact your guardian to book a trip on your behalf.");
            }

            // Check if the passenger is already in a trip that hasn't completed
            //var ongoingTrip = await _unitOfWork.TripRepository.GetOngoingTripByPassengerId(userId);
            //if (ongoingTrip != null)
            //{
            //    throw new BadRequestException("Passenger is already in a trip that hasn't completed. Please complete the current trip before creating a new one.");
            //}

            // Prevent users who are busy from creating new trip
            if (booker.Status == UserStatus.BUSY)
            {
                if (booker.Isdriver)
                {
                    throw new BadRequestException("You are not allow to create trip at the moment.");
                }
                throw new BadRequestException("You cannot create more trip.");
            }

            var now = DateTimeUtilities.GetDateTimeVnNow();
            var cancellationWindowMinutes = _settingService.GetSetting("TRIP_CANCELLATION_WINDOW");
            //var cancellationLimit = _settingService.GetSetting("TRIP_CANCELLATION_LIMIT");

            //var cancellationWindow = now.AddMinutes(-cancellationWindowMinutes);
            
            //the booker is ban because of spam
            if (booker.CancellationBanUntil != null)
            {
                // Check if the user has cancelled too many trips recently (being banned)
                if (now < booker.CancellationBanUntil)
                {
                    throw new BadRequestException($"You have exceeded the maximum number of cancellations allowed within {cancellationWindowMinutes} minutes. " +
                        $"Please wait until {booker.CancellationBanUntil} before creating a new trip.");
                }
                else
                {
                    // If the ban duration has passed, reset
                    booker.CanceledTripCount = 0;
                    booker.LastTripCancellationTime = null;
                    booker.CancellationBanUntil = null;
                    booker.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    await _unitOfWork.UserRepository.UpdateAsync(booker);
                }
            }

            // Check if a past origin with the same coordinates already exists
            var departure = await _unitOfWork.LocationRepository.GetByUserIdAndLatLongAndTypeAsync(userId, request.StartLatitude, request.StartLongitude, LocationType.PAST_ORIGIN);

            if (departure == null)
            {
                // If not, create a new past origin location
                departure = new Location
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Address = request.StartAddress,
                    Latitude = request.StartLatitude,
                    Longtitude = request.StartLongitude,
                    Type = LocationType.PAST_ORIGIN,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.LocationRepository.AddAsync(departure);
            }
            else
            {
                // If a past origin with the same coordinates exists, update its information
                departure.Address = request.StartAddress;
                departure.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

                await _unitOfWork.LocationRepository.UpdateAsync(departure);
            }

            // Same for destination
            var destination = await _unitOfWork.LocationRepository.GetByUserIdAndLatLongAndTypeAsync(userId, request.EndLatitude, request.EndLongitude, LocationType.PAST_DESTINATION);
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
            else
            {
                destination.Address = request.EndAddress;
                destination.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

                await _unitOfWork.LocationRepository.UpdateAsync(destination);
            }

            var distance = await GoogleMapsApiUtilities.ComputeDistanceMatrixAsync(departure, destination);

            var totalPrice = await _unitOfWork.CartypeRepository.CalculatePriceForCarType(request.CartypeId, distance);

            // Check if user's wallet has enough balance to book trip
            if (request.PaymentMethod == PaymentMethod.WALLET)
            {
                Guid walletOwnerId = booker.Id;

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
                StartLocationId = departure.Id,
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
                Note = request.Note,
                PassengerName = request.DependentInfo.Name,
                PassengerPhoneNumber = request.DependentInfo.Phone ?? null,
                Type = TripType.BOOK_FOR_DEP_NO_APP
            };

            await _unitOfWork.TripRepository.AddAsync(trip);

            //booker.Status = UserStatus.BUSY;
            booker.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
            await _unitOfWork.UserRepository.UpdateAsync(booker);

            await _unitOfWork.Save();

            tripDto = _mapper.Map<TripDto>(trip);

            // Background task
            string jobId = BackgroundJob.Enqueue<BackgroundServices>(s => s.FindDriver(trip.Id, request.CartypeId));

            return tripDto;
        }
    }
}
