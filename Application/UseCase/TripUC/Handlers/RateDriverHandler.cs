﻿using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Application.UseCase.TripUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.TripUC.Handlers
{
    public class RateDriverHandler : IRequestHandler<RateDriverCommand, RatingDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;
        private readonly ISettingService _settingService;

        public RateDriverHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims userClaims, ISettingService settingService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClaims = userClaims;
            _settingService = settingService;
        }

        public async Task<RatingDto> Handle(RateDriverCommand request, CancellationToken cancellationToken)
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

            if (trip.DriverId == null)
            {
                throw new BadRequestException("There's no driver for this trip.");
            }

            // Check if a rating already exists for this user and trip
            var existingRating = await _unitOfWork.RatingRepository.GetRatingByUserAndTrip(userId, request.TripId);
            if (existingRating != null)
            {
                throw new BadRequestException("You have already rated this trip.");
            }

            var driver = await _unitOfWork.UserRepository.GetUserById(trip.DriverId.Value.ToString());
            if (driver == null)
            {
                throw new NotFoundException(nameof(User), trip.DriverId.Value);
            }

            if (trip.PassengerId != userId && trip.BookerId != userId)
            {
                throw new BadRequestException("User does not match for this trip.");
            }

            if (trip.Status != TripStatus.COMPLETED)
            {
                throw new BadRequestException("The trip is invalid.");
            }

            var rating = new Rating
            {
                Id = Guid.NewGuid(),
                Rater = userId,
                Ratee = trip.DriverId.Value,
                TripId = request.TripId,
                RatingValue = request.Rating,
                Comment = request.Comment,
                CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
            };

            await _unitOfWork.RatingRepository.AddAsync(rating);

            driver.TotalRating += request.Rating;
            driver.RatingCount++;

            // Check rating
            var ratingThreshold = _settingService.GetSetting("RATING_THRESHOLD");
            var warningDuration = _settingService.GetSetting("WARNING_DURATION");

            // Issue a warning 
            if (driver.RatingStatus == RatingStatus.GOOD && driver.WarnedTime == null && driver.AverageRating < ratingThreshold)
            {
                driver.RatingStatus = RatingStatus.WARNED;
                driver.WarnedTime = DateTimeUtilities.GetDateTimeVnNow();
                driver.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
            }
            else if (driver.RatingStatus == RatingStatus.WARNED && driver.WarnedTime != null && driver.AverageRating >= ratingThreshold)
            {
                // If driver's rating status is WARNED and their average rating is above the threshold, change their status back to GOOD
                driver.RatingStatus = RatingStatus.GOOD;
                driver.WarnedTime = null;
                driver.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
            }

            await _unitOfWork.UserRepository.UpdateAsync(driver);

            await _unitOfWork.Save();

            return _mapper.Map<RatingDto>(rating);
        }
    }
}
