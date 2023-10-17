using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Service;
using Application.Services;
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

            //ClaimsPrincipal claims = _tokenService.ValidateToken(Token ?? "");
            ClaimsPrincipal? claims = _tokenService.ValidateToken("");
            if (claims != null)
            {
                Guid.TryParse(claims.FindFirst("jti")?.Value, out Guid userId);

                var startLatitude = decimal.Parse(request.StartLatitude!);
                var startLongitude = decimal.Parse(request.StartLongitude!);
                var endLatitude = decimal.Parse(request.EndLatitude!);
                var endLongitude = decimal.Parse(request.EndLongitude!);

                var origin = await _unitOfWork.LocationRepository.GetByUserIdAndTypeAsync(userId, LocationType.CURRENT_LOCATION);
                if (origin == null)
                {
                    origin = new Location
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Latitude = startLatitude,
                        Longtitude = startLongitude,
                        Type = LocationType.CURRENT_LOCATION,
                        CreateTime = DateTime.Now,
                        UpdatedTime = DateTime.Now
                    };

                    await _unitOfWork.LocationRepository.AddAsync(origin);
                }
                else
                {
                    origin.Latitude = startLatitude;
                    origin.Longtitude = startLongitude;
                    origin.UpdatedTime = DateTime.Now;

                    await _unitOfWork.LocationRepository.UpdateAsync(origin);
                }

                var destination = new Location
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Latitude = endLatitude,
                    Longtitude = endLongitude,
                    Type = LocationType.PAST_DESTINATION,
                    CreateTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                };

                await _unitOfWork.LocationRepository.AddAsync(destination);

                var trip = new Trip
                {
                    Id = Guid.NewGuid(),
                    PassengerId = userId,
                    StartLocationId = origin.Id,
                    EndLocationId = destination.Id,
                    StartTime = DateTime.Now,
                    Status = TripStatus.PENDING
                };

                await _unitOfWork.TripRepository.AddAsync(trip);

                tripDto = _mapper.Map<TripDto>(trip);

                // Background task
                BackgroundJob.Enqueue<BackgroundServices>(s => s.FindDriver(trip.Id));
            }
            else
            {
                throw new BadRequestException("Invalid credentials");
            }

            return tripDto;
        }
    }
}
