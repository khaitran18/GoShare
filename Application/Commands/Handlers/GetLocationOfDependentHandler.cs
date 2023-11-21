using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Application.SignalR;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class GetLocationOfDependentHandler : IRequestHandler<GetLocationOfDependentCommand, LocationDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly UserClaims _userClaims;
        private readonly IMapper _mapper;

        public GetLocationOfDependentHandler(IUnitOfWork unitOfWork, IHubContext<SignalRHub> hubContext, UserClaims userClaims, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _userClaims = userClaims;
            _mapper = mapper;
        }

        public async Task<LocationDto> Handle(GetLocationOfDependentCommand request, CancellationToken cancellationToken)
        {
            var locationDto = new LocationDto();

            Guid userId = (Guid)_userClaims.id!;

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

            // Request the location from the dependent's device
            await _hubContext.Clients.Group(request.DependentId.ToString()).SendAsync("RequestLocation");

            await Task.Delay(TimeSpan.FromSeconds(5));

            // Return the obtained location data
            var dependentLocation = KeyValueStore.Instance.Get<string>($"CurrentLocation_{request.DependentId}");
            if (string.IsNullOrEmpty(dependentLocation))
            {
                throw new BadRequestException("Unable to get dependent's location");
            }

            var dependentLocationData = JsonConvert.DeserializeObject<LocationData>(dependentLocation!);
            if (dependentLocationData == null)
            {
                throw new Exception("Location data is null.");
            }

            var location = await _unitOfWork.LocationRepository.GetByUserIdAndTypeAsync(request.DependentId, LocationType.CURRENT_LOCATION);
            if (location == null)
            {
                location = new Location
                {
                    Id = Guid.NewGuid(),
                    UserId = request.DependentId,
                    Address = dependentLocationData.Address,
                    Latitude = dependentLocationData.Latitude,
                    Longtitude = dependentLocationData.Longitude,
                    Type = LocationType.CURRENT_LOCATION,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.LocationRepository.AddAsync(location);
            }
            else
            {
                location.Address = dependentLocationData.Address;
                location.Latitude = dependentLocationData.Latitude;
                location.Longtitude = dependentLocationData.Longitude;
                location.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

                await _unitOfWork.LocationRepository.UpdateAsync(location);
            }

            await _unitOfWork.Save();

            locationDto = _mapper.Map<LocationDto>(location);

            return locationDto;
        }
    }
}
