﻿using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services;
using Application.Services.Interfaces;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class CancelTripHandler : IRequestHandler<CancelTripCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ISettingService _settingService;

        public CancelTripHandler(IUnitOfWork unitOfWork, ITokenService tokenService, ISettingService settingService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _settingService = settingService;
        }

        public async Task<bool> Handle(CancelTripCommand request, CancellationToken cancellationToken)
        {
            ClaimsPrincipal? claims = _tokenService.ValidateToken(request.Token ?? "");
            Guid.TryParse(claims!.FindFirst("id")?.Value, out Guid userId);

            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            var now = DateTime.Now;

            var trip = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);
            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), request.TripId);
            }

            if (trip.PassengerId != userId)
            {
                throw new BadRequestException("The passenger does not match for this trip.");
            }

            if (trip.Status != TripStatus.PENDING)
            {
                throw new BadRequestException("The trip is invalid.");
            }

            trip.Status = TripStatus.CANCELED;
            trip.UpdatedTime = DateTime.Now;

            await _unitOfWork.TripRepository.UpdateAsync(trip);

            user.CanceledTripCount++;

            // Cancel reach limit
            var cancellationLimit = _settingService.GetSetting("TRIP_CANCELLATION_LIMIT");
            var banDuration = _settingService.GetSetting("CANCELLATION_BAN_DURATION");
            var cancellationWindowMinutes = _settingService.GetSetting("TRIP_CANCELLATION_WINDOW");

            if (user.CanceledTripCount == 1)
            {
                user.LastTripCancellationTime = now;

                // Scheduled job to reset cancellation count and time
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
                    await _unitOfWork.UserRepository.UpdateAsync(driver);
                }
            }

            await _unitOfWork.Save();

            // Cancel find driver task
            string jobId = KeyValueStore.Instance.Get<string>($"FindDriverTask_{trip.Id}");
            if (!string.IsNullOrEmpty(jobId))
            {
                BackgroundJob.Delete(jobId);
                KeyValueStore.Instance.Remove($"FindDriverTask_{trip.Id}");
            }

            return true;
        }
    }
}
