using Application.Common.Dtos;
using Application.UseCase.LocationUC.Queries;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.LocationUC.Handlers
{
    public class GetLocationQueryHandler : IRequestHandler<GetLocationQuery, List<LocationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _claims;
        private readonly IMapper _mapper;

        public GetLocationQueryHandler(IUnitOfWork unitOfWork, UserClaims claims, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _claims = claims;
            _mapper = mapper;
        }

        public async Task<List<LocationDto>> Handle(GetLocationQuery request, CancellationToken cancellationToken)
        {
            List<Location> list = await _unitOfWork.LocationRepository.GetListByUserIdAndTypeAsync((Guid)_claims.id!, Domain.Enumerations.LocationType.PLANNED_DESTINATION);

            // Get the list of past completed trips
            List<Trip> pastTrips = await _unitOfWork.TripRepository.GetPastCompletedTripsByPassengerIdAsync((Guid)_claims.id!);

            // Group the past trips by end location and count the frequency
            var locationFrequencies = pastTrips.GroupBy(t => t.EndLocationId)
                                               .ToDictionary(g => g.Key, g => g.Count());

            // Sort the planned destinations by frequency, then by CreateTime
            list.Sort((a, b) =>
            {
                int frequencyComparison = locationFrequencies.GetValueOrDefault(b.Id, 0).CompareTo(locationFrequencies.GetValueOrDefault(a.Id, 0));
                if (frequencyComparison != 0)
                {
                    return frequencyComparison;
                }
                else
                {
                    // If frequencies are equal, sort by CreateTime (most recent first)
                    return b.CreateTime.CompareTo(a.CreateTime);
                }
            });

            List<LocationDto> response = _mapper.Map<List<LocationDto>>(list);
            return response;
        }
    }
}
