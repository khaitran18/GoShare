using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Hangfire;
using MediatR;
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

        public CancelTripHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
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
            var tenMinutesAgo = now.AddMinutes(-10);

            // Check if the user has cancelled too many trips recently
            if (user.LastTripCancellationTime >= tenMinutesAgo && user.CanceledTripCount >= 20)
            {
                throw new BadRequestException("You have exceeded the maximum number of cancellations allowed within 10 minutes.");
            }

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

            // If the last cancellation was more than 10 minutes ago, reset the count
            if (user.LastTripCancellationTime < tenMinutesAgo)
            {
                user.CanceledTripCount = 0;
            }

            user.CanceledTripCount++;
            user.LastTripCancellationTime = now;

            await _unitOfWork.UserRepository.UpdateAsync(user);

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
