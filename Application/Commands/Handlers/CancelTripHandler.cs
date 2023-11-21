using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class CancelTripHandler : IRequestHandler<CancelTripCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettingService _settingService;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;
        private readonly ILogger<BackgroundServices> _logger;

        public CancelTripHandler(IUnitOfWork unitOfWork, ISettingService settingService, IMapper mapper, UserClaims userClaims, ILogger<BackgroundServices> logger)
        {
            _unitOfWork = unitOfWork;
            _settingService = settingService;
            _mapper = mapper;
            _userClaims = userClaims;
            _logger = logger;
        }

        public async Task<TripDto> Handle(CancelTripCommand request, CancellationToken cancellationToken)
        {
            Guid userId = (Guid)_userClaims.id!;

            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            var trip = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);
            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), request.TripId);
            }

            if (trip.PassengerId != userId && trip.BookerId != userId)
            {
                throw new BadRequestException("User does not match for this trip.");
            }

            if (trip.Status != TripStatus.PENDING)
            {
                throw new BadRequestException("The trip is invalid.");
            }

            var now = DateTimeUtilities.GetDateTimeVnNow();
            var cancellationWindowMinutes = _settingService.GetSetting("TRIP_CANCELLATION_WINDOW");
            var cancellationLimit = _settingService.GetSetting("TRIP_CANCELLATION_LIMIT");

            var cancellationWindow = now.AddMinutes(-cancellationWindowMinutes);

            // Check if the last cancellation was long ago
            if (user.CanceledTripCount > 1 && user.CanceledTripCount < cancellationLimit && user.LastTripCancellationTime < cancellationWindow)
            {
                user.CanceledTripCount = 0;
                user.LastTripCancellationTime = null;
                user.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                await _unitOfWork.UserRepository.UpdateAsync(user);
            }

            // Check if the user has cancelled too many trips recently (this is only for blocking dependent)
            if (user.LastTripCancellationTime >= cancellationWindow && user.CanceledTripCount >= cancellationLimit)
            {
                if (now < user.CancellationBanUntil)
                {
                    throw new BadRequestException($"You have exceeded the maximum number of cancellations allowed within {cancellationWindowMinutes} minutes.");
                }
                else
                {
                    // If the ban duration has passed, reset
                    user.CanceledTripCount = 0;
                    user.LastTripCancellationTime = null;
                    user.CancellationBanUntil = null;
                    user.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    await _unitOfWork.UserRepository.UpdateAsync(user);
                }
            }

            trip.Status = TripStatus.CANCELED;
            trip.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

            await _unitOfWork.TripRepository.UpdateAsync(trip);

            user.CanceledTripCount++;

            // Cancel reach limit
            var banDuration = _settingService.GetSetting("CANCELLATION_BAN_DURATION");

            if (user.CanceledTripCount == 1)
            {
                user.LastTripCancellationTime = now;

                // Scheduled job to reset cancellation count and time
                _logger.LogInformation("Scheduling job to reset trip cancellation for userId: {userId}", user.Id);
                var resetJobId = BackgroundJob.Schedule<BackgroundServices>(s => s.ResetCancellationCountAndTime(user.Id), TimeSpan.FromMinutes(cancellationWindowMinutes));
                KeyValueStore.Instance.Set($"ResetCancellationTask_{user.Id}", resetJobId);
            }

            if (user.CanceledTripCount == cancellationLimit)
            {
                user.CancellationBanUntil = now.AddMinutes(banDuration);

                // Cancel the reset job as the user has reached the cancellation limit
                var resetJobId = KeyValueStore.Instance.Get<string>($"ResetCancellationTask_{user.Id}");
                if (!string.IsNullOrEmpty(resetJobId))
                {
                    _logger.LogInformation("Cancelling reset job for userId: {userId} as cancellation limit has been reached", user.Id);
                    BackgroundJob.Delete(resetJobId);
                    KeyValueStore.Instance.Remove($"ResetCancellationTask_{user.Id}");
                }
            }

            await _unitOfWork.UserRepository.UpdateAsync(user);

            // Check if a driver was assigned to the trip
            if (trip.DriverId.HasValue)
            {
                var driver = await _unitOfWork.UserRepository.GetUserById(trip.DriverId.Value.ToString());
                if (driver != null && driver.Status == UserStatus.BUSY)
                {
                    // Reset the status of the driver to active
                    driver.Status = UserStatus.ACTIVE;
                    driver.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    await _unitOfWork.UserRepository.UpdateAsync(driver);

                    trip.DriverId = null;
                    trip.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    await _unitOfWork.TripRepository.UpdateAsync(trip);
                }
            }

            // Change status of passenger back to inactive
            trip.Passenger.Status = UserStatus.INACTIVE;
            trip.Passenger.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
            await _unitOfWork.UserRepository.UpdateAsync(user);

            await _unitOfWork.Save();

            // Cancel find driver task
            _logger.LogInformation("Cancelling find driver task for tripId: {tripId}", trip.Id);
            KeyValueStore.Instance.Set($"CancelFindDriverTask_{trip.Id}", "true");

            return _mapper.Map<TripDto>(trip);
        }
    }
}