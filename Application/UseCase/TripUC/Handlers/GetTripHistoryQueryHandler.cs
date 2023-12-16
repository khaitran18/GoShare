﻿using Application.Common.Dtos;
using Application.UseCase.TripUC.Queries;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase.TripUC.Handlers
{
    public class GetTripHistoryQueryHandler : IRequestHandler<GetTripHistoryQuery, List<TripDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;

        public GetTripHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClaims = userClaims;
        }

        public async Task<List<TripDto>> Handle(GetTripHistoryQuery request, CancellationToken cancellationToken)
        {
            Guid userId = (Guid)_userClaims.id!;
            var trips = await _unitOfWork.TripRepository.GetTripHistoryByUserId(userId);

            var tripDtos = _mapper.Map<List<TripDto>>(trips);

            return tripDtos;
        }
    }
}