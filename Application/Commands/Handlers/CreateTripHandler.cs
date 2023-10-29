using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities.Google;
using Application.Service;
using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
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
    public class CreateTripHandler : IRequestHandler<CreateTripCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public CreateTripHandler(IUnitOfWork unitOfWork, IMapper mapper, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<TripDto> Handle(CreateTripCommand request, CancellationToken cancellationToken)
        {
            var tripDto = new TripDto();

            ClaimsPrincipal? claims = _tokenService.ValidateToken(request.Token ?? "");
            Guid.TryParse(claims!.FindFirst("id")?.Value, out Guid userId);

            var passenger = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (passenger == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            Guid walletOwnerId = passenger.GuardianId ?? passenger.Id;

            var walletOwnerWallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(walletOwnerId);
            if (walletOwnerWallet == null)
            {
                throw new NotFoundException(nameof(Wallet), walletOwnerId);
            }

            var origin = await _unitOfWork.LocationRepository.GetByUserIdAndTypeAsync(userId, LocationType.CURRENT_LOCATION);
            if (origin == null)
            {
                origin = new Location
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Address = request.StartAddress,
                    Latitude = request.StartLatitude,
                    Longtitude = request.StartLongitude,
                    Type = LocationType.CURRENT_LOCATION,
                    CreateTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                };

                await _unitOfWork.LocationRepository.AddAsync(origin);
            }
            else
            {
                origin.Address = request.StartAddress;
                origin.Latitude = request.StartLatitude;
                origin.Longtitude = request.StartLongitude;
                origin.UpdatedTime = DateTime.Now;

                await _unitOfWork.LocationRepository.UpdateAsync(origin);
            }

            var destination = new Location
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Address = request.EndAddress,
                Latitude = request.EndLatitude,
                Longtitude = request.EndLongitude,
                Type = LocationType.PAST_DESTINATION,
                CreateTime = DateTime.Now,
                UpdatedTime = DateTime.Now
            };

            var distance = await GoogleMapsApiUtilities.ComputeDistanceMatrixAsync(origin, destination);

            var totalPrice = await _unitOfWork.CartypeRepository.CalculatePriceForCarType(request.CartypeId, distance);

            if (walletOwnerWallet.Balance < totalPrice)
            {
                throw new Exception("The wallet owner's wallet does not have enough balance.");
            }

            await _unitOfWork.LocationRepository.AddAsync(destination);

            var trip = new Trip
            {
                Id = Guid.NewGuid(),
                PassengerId = userId,
                StartLocationId = origin.Id,
                EndLocationId = destination.Id,
                StartTime = DateTime.Now,
                CreateTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                Distance = distance,
                CartypeId = request.CartypeId,
                Price = totalPrice,
                Status = TripStatus.PENDING
            };

            await _unitOfWork.TripRepository.AddAsync(trip);

            tripDto = _mapper.Map<TripDto>(trip);

            // Background task
            BackgroundJob.Enqueue<BackgroundServices>(s => s.FindDriver(trip.Id, request.CartypeId));

            return tripDto;
        }
    }
}
