using Application.Common.Dtos;
using Application.UseCase.TripUC.Queries;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.TripUC.Handlers
{
    public class GetTripsHandler : IRequestHandler<GetTripsQuery, PaginatedResult<TripDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTripsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<TripDto>> Handle(GetTripsQuery request, CancellationToken cancellationToken)
        {
            var (trips, totalCount) = await _unitOfWork.TripRepository.GetTrips(
                request.Status,
                request.PaymentMethod,
                request.Type,
                request.SortBy,
                request.Page,
                request.PageSize
            );

            var tripDtos = _mapper.Map<List<TripDto>>(trips);

            return new PaginatedResult<TripDto>(
                tripDtos,
                totalCount,
                request.Page,
                request.PageSize
            );
        }
    }
}
