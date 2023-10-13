using Application.Common.Dtos;
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

namespace Application.Commands.Handler
{
    public class CreateTripHandler : IRequestHandler<CreateTripCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTripHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TripDto> Handle(CreateTripCommand request, CancellationToken cancellationToken)
        {
            var startLatitude = double.Parse(request.StartLatitude!);
            var startLongitude = double.Parse(request.StartLongitude!);
            var endLatitude = double.Parse(request.EndLatitude!);
            var endLongitude = double.Parse(request.EndLongitude!);

            var origin = new Location
            {
                Id = Guid.NewGuid(),
                //UserId = request.PassengerId,
                Latitude = startLatitude,
                Longitude = startLongitude,
                Type = LocationType.CURRENT_LOCATION,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now
            };

            await _unitOfWork.LocationRepository.AddAsync(origin);

            var destination = new Location
            {
                Id = Guid.NewGuid(),
                //UserId = request.PassengerId,
                Latitude = endLatitude,
                Longitude = endLongitude,
                Type = LocationType.PAST_DESTINATION,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now
            };

            await _unitOfWork.LocationRepository.AddAsync(destination);

            var trip = new Trip
            {
                Id = Guid.NewGuid(),
                //PassengerId = request.PassengerId,
                StartLocationId = origin.Id,
                EndLocationId = destination.Id,
                StartTime = DateTime.Now,
                Status = TripStatus.PENDING
            };

            await _unitOfWork.TripRepository.AddAsync(trip);

            var tripDto = _mapper.Map<TripDto>(trip);

            // Background task
            BackgroundJob.Enqueue<BackgroundServices>(s => s.FindDriver(trip.Id));

            return tripDto;
        }
    }
}
