using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Service;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class EndTripHandler : IRequestHandler<EndTripCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public EndTripHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<TripDto> Handle(EndTripCommand request, CancellationToken cancellationToken)
        {
            var tripDto = new TripDto();

            ClaimsPrincipal? claims = _tokenService.ValidateToken(request.Token ?? "");
            var trip = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);

            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), request.TripId);
            }

            trip.Status = TripStatus.COMPLETED;
            trip.EndTime = DateTime.Now;
            trip.UpdatedTime = DateTime.Now;

            await _unitOfWork.TripRepository.UpdateAsync(trip);

            tripDto = _mapper.Map<TripDto>(trip);

            return tripDto;
        }
    }

}
