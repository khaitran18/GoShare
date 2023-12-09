using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
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

namespace Application.Commands.Handlers
{
    public class CreatePlannedDestinationHandler : IRequestHandler<CreatePlannedDestinationCommand, LocationDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;

        public CreatePlannedDestinationHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClaims = userClaims;
        }

        public async Task<LocationDto> Handle(CreatePlannedDestinationCommand request, CancellationToken cancellationToken)
        {
            var plannedDestinationDto = new LocationDto();

            Guid userId = (Guid)_userClaims.id!;
            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            var plannedDestination = new Location
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Latitude = request.Latitude,
                Longtitude = request.Longitude,
                Address = request.Address,
                Type = LocationType.PLANNED_DESTINATION,
                CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
            };

            //if (request.UserId == null)
            //{
            //    // Create location for self
            //    plannedDestination.UserId = userId;
            //}
            //else
            //{
            //    var dependent = await _unitOfWork.UserRepository.GetUserById(request.UserId.ToString()!);
            //    if (dependent == null)
            //    {
            //        throw new NotFoundException(nameof(User), request.UserId);
            //    }

            //    // Check if dependent is of the user
            //    if (dependent.GuardianId != userId)
            //    {
            //        throw new BadRequestException("The user is not the guardian of the dependent.");
            //    }

            //    plannedDestination.UserId = (Guid)request.UserId;
            //}

            plannedDestination.UserId = userId;

            // Check if a planned destination with the same coordinates already exists
            var existingPlannedDestination = await _unitOfWork.LocationRepository.GetByUserIdAndLatLongAndTypeAsync(plannedDestination.UserId, request.Latitude, request.Longitude, LocationType.PLANNED_DESTINATION);

            if (existingPlannedDestination != null)
            {
                throw new BadRequestException("A planned destination with the same coordinates already exists.");
            }

            await _unitOfWork.LocationRepository.AddAsync(plannedDestination);
            await _unitOfWork.Save();

            plannedDestinationDto = _mapper.Map<LocationDto>(plannedDestination);

            return plannedDestinationDto;
        }
    }
}
