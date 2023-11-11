using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Common.Utilities.Google;
using Application.Services;
using Application.Services.Interfaces;
using Application.SignalR;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class CreateTripForDependentHandler : IRequestHandler<CreateTripForDependentCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISettingService _settingService;
        private readonly IHubContext<SignalRHub> _hubContext;

        public CreateTripForDependentHandler(IUnitOfWork unitOfWork, IMapper mapper, ITokenService tokenService, IServiceProvider serviceProvider, ISettingService settingService, IHubContext<SignalRHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
            _serviceProvider = serviceProvider;
            _settingService = settingService;
            _hubContext = hubContext;
        }

        public async Task<TripDto> Handle(CreateTripForDependentCommand request, CancellationToken cancellationToken)
        {
            var tripDto = new TripDto();

            ClaimsPrincipal? claims = _tokenService.ValidateToken(request.Token ?? "");
            Guid.TryParse(claims!.FindFirst("id")?.Value, out Guid userId);

            var guardian = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (guardian == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }
            // Check if user is a guardian
            if (guardian.GuardianId != null)
            {
                throw new BadRequestException("You are not allow to do this function");
            }

            var dependent = await _unitOfWork.UserRepository.GetUserById(request.DependentId.ToString());
            if (dependent == null)
            {
                throw new NotFoundException(nameof(User), request.DependentId);
            }
            // Check if dependent is of the user
            if (dependent == null || dependent.GuardianId != userId)
            {
                throw new BadRequestException("The user is not the guardian of the dependent.");
            }

            var now = DateTime.Now;
            var cancellationWindowMinutes = _settingService.GetSetting("TRIP_CANCELLATION_WINDOW");
            var cancellationLimit = _settingService.GetSetting("TRIP_CANCELLATION_LIMIT");

            var cancellationWindow = now.AddMinutes(-cancellationWindowMinutes);

            // Check if the user has cancelled too many trips recently (guardian)
            if (guardian.LastTripCancellationTime >= cancellationWindow && guardian.CanceledTripCount >= cancellationLimit)
            {
                if (now < guardian.CancellationBanUntil)
                {
                    throw new BadRequestException($"You have exceeded the maximum number of cancellations allowed within {cancellationWindowMinutes} minutes. Please wait until {guardian.CancellationBanUntil} before creating a new trip.");
                }
                else
                {
                    // If the ban duration has passed, reset
                    guardian.CanceledTripCount = 0;
                    guardian.LastTripCancellationTime = null;
                    guardian.CancellationBanUntil = null;
                    await _unitOfWork.UserRepository.UpdateAsync(guardian);
                }
            }

            var currentLocation = await _unitOfWork.LocationRepository.GetByUserIdAndTypeAsync(request.DependentId, LocationType.CURRENT_LOCATION);
            if (currentLocation == null)
            {
                throw new NotFoundException(nameof(Location), request.DependentId);
            }

            var pastOrigin = new Location
            {
                Id = Guid.NewGuid(),
                UserId = request.DependentId,
                Address = currentLocation.Address,
                Latitude = currentLocation.Latitude,
                Longtitude = currentLocation.Longtitude,
                Type = LocationType.PAST_ORIGIN,
                CreateTime = DateTime.Now,
                UpdatedTime = DateTime.Now
            };

            await _unitOfWork.LocationRepository.AddAsync(pastOrigin);

            var destination = await _unitOfWork.LocationRepository.GetByUserIdAndLatLongAsync(request.DependentId, request.EndLatitude, request.EndLongitude);
            if (destination == null)
            {
                destination = new Location
                {
                    Id = Guid.NewGuid(),
                    UserId = request.DependentId,
                    Address = request.EndAddress,
                    Latitude = request.EndLatitude,
                    Longtitude = request.EndLongitude,
                    Type = LocationType.PAST_DESTINATION,
                    CreateTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                };

                await _unitOfWork.LocationRepository.AddAsync(destination);
            }

            var distance = await GoogleMapsApiUtilities.ComputeDistanceMatrixAsync(pastOrigin, destination);

            var totalPrice = await _unitOfWork.CartypeRepository.CalculatePriceForCarType(request.CartypeId, distance);

            if (request.PaymentMethod == PaymentMethod.WALLET)
            {
                var walletOwnerWallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);
                if (walletOwnerWallet == null)
                {
                    throw new NotFoundException(nameof(Wallet), userId);
                }

                if (walletOwnerWallet.Balance < totalPrice)
                {
                    throw new BadRequestException("The wallet owner's wallet does not have enough balance.");
                }
            }

            var trip = new Trip
            {
                Id = Guid.NewGuid(),
                PassengerId = request.DependentId,
                StartLocationId = pastOrigin.Id,
                EndLocationId = destination.Id,
                StartTime = DateTime.Now,
                CreateTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
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
            var cts = _serviceProvider.GetRequiredService<CancellationTokenSource>();
            string jobId = BackgroundJob.Enqueue<BackgroundServices>(s => s.FindDriver(trip.Id, request.CartypeId, cts.Token));
            KeyValueStore.Instance.Set($"FindDriverTask_{trip.Id}", jobId);

            return tripDto;
        }
    }
}
