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

namespace Application.Commands.Handlers
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
            var startLatitude = decimal.Parse(request.StartLatitude!);
            var startLongitude = decimal.Parse(request.StartLongitude!);
            var endLatitude = decimal.Parse(request.EndLatitude!);
            var endLongitude = decimal.Parse(request.EndLongitude!);

            var origin = new Location
            {
                Id = Guid.NewGuid(),
                //UserId = request.PassengerId,
                Latitude = startLatitude,
                Longtitude = startLongitude,
                Type = LocationType.CURRENT_LOCATION,
                CreateTime = DateTime.Now,
                UpdatedTime = DateTime.Now
            };

            await _unitOfWork.LocationRepository.AddAsync(origin);

            var destination = new Location
            {
                Id = Guid.NewGuid(),
                //UserId = request.PassengerId,
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
