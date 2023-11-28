﻿using Application.Common.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Handler
{
    public class GetTripHistoryHandler : IRequestHandler<GetTripHistoryQuery, List<TripDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;

        public GetTripHistoryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClaims = userClaims;
        }

        public async Task<List<TripDto>> Handle(GetTripHistoryQuery request, CancellationToken cancellationToken)
        {
            Guid userId = (Guid)_userClaims.id!;
            var trips = await _unitOfWork.TripRepository.GetTripsByUserId(userId, request.SortBy);

            var tripDtos = _mapper.Map<List<TripDto>>(trips);

            return tripDtos;
        }
    }
}