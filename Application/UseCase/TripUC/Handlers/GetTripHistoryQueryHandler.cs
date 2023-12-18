using Application.Common.Dtos;
using Application.UseCase.TripUC.Queries;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
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
            var userRole = _userClaims.Role;
            var trips = new List<Trip>();

            if (userRole == UserRoleEnumerations.Driver)
            {
                trips = await _unitOfWork.TripRepository.GetTripHistoryByDriverId(userId);
            }
            else if (userRole == UserRoleEnumerations.Dependent)
            {
                trips = await _unitOfWork.TripRepository.GetTripHistoryByPassengerId(userId);
            }
            else if (userRole == UserRoleEnumerations.User)
            {
                trips = await _unitOfWork.TripRepository.GetTripHistoryByBookerId(userId);
            }

            var tripDtos = _mapper.Map<List<TripDto>>(trips);

            return tripDtos;
        }
    }
}