using Application.Common.Dtos;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Handler
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

        public Task<List<LocationDto>> Handle(GetLocationQuery request, CancellationToken cancellationToken)
        {
            List<LocationDto> response = new List<LocationDto>();
            List<Location> list = _unitOfWork.LocationRepository.GetListByUserIdAndTypeAsync((Guid)_claims.id!, Domain.Enumerations.LocationType.PLANNED_DESTINATION).Result;
            response = _mapper.Map<List<LocationDto>>(list);
            return Task.FromResult(response);
        }
    }
}
