using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Services.Interfaces;
using Application.UseCase.TripUC.Queries;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.TripUC.Handlers
{
    public class GetTripQueryHandler : IRequestHandler<GetTripQuery, TripDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _claims;
        private readonly IUserService _userService;

        public GetTripQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims claims, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claims = claims;
            _userService = userService;
        }

        public async Task<TripDto?> Handle(GetTripQuery request, CancellationToken cancellationToken)
        {
            TripDto? response = new TripDto();
            Trip? t = await _unitOfWork.TripRepository.GetByIdAsync(new Guid(request.TripId));
            //if trip exist
            if (t is not null)
            {
                //if user is booker of the trip
                if (!_claims.id.Equals(t.BookerId))
                {
                    //if driver is driver of the trip
                    if (!_claims.id.Equals(t.DriverId))
                    {
                        //if user is passenger dependent
                        if (!await _userService.CheckDependentStatus(_unitOfWork, t.PassengerId, (Guid)_claims.id!))
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }
                }
            }
            else throw new NotFoundException("Trip is not found");
            response = _mapper.Map<TripDto>(t);
            if (response.Driver is not null)
            {
                response.Driver!.Car = _mapper.Map<CarDto>(await _unitOfWork.CarRepository.GetByUserId((Guid)t.DriverId!));
            }
            return response;
        }
    }
}
