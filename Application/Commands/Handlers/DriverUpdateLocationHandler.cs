using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
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
    public class DriverUpdateLocationHandler : IRequestHandler<DriverUpdateLocationCommand, LocationDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _userClaims;
        private readonly IMapper _mapper;

        public DriverUpdateLocationHandler(IUnitOfWork unitOfWork, UserClaims userClaims, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userClaims = userClaims;
            _mapper = mapper;
        }

        public async Task<LocationDto> Handle(DriverUpdateLocationCommand request, CancellationToken cancellationToken)
        {
            var locationDto = new LocationDto();
            Guid userId = (Guid)_userClaims.id!;

            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            if (user.Status == UserStatus.BUSY)
            {
                throw new BadRequestException("You cannot update your location at the moment.");
            }

            var location = await _unitOfWork.LocationRepository.GetByUserIdAndTypeAsync(userId, LocationType.CURRENT_LOCATION);
            if (location == null)
            {
                throw new NotFoundException(nameof(Location), userId);
            }

            location.Latitude = request.Latitude;
            location.Longtitude = request.Longitude;
            location.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

            await _unitOfWork.LocationRepository.UpdateAsync(location);
            await _unitOfWork.Save();

            locationDto = _mapper.Map<LocationDto>(location);

            return locationDto;
        }
    }
}
