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
    public class GetCurrentTripQueryHandler : IRequestHandler<GetCurrentTripQuery, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _claims;

        public GetCurrentTripQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims claims)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claims = claims;
        }

        public Task<TripDto> Handle(GetCurrentTripQuery request, CancellationToken cancellationToken)
        {
            var response = new TripDto();
            Trip? t = _unitOfWork.TripRepository.GetOngoingTripByPassengerId((Guid)_claims.id!).Result;
            response = _mapper.Map<TripDto>(t);
            return Task.FromResult(response);
        }
    }
}
