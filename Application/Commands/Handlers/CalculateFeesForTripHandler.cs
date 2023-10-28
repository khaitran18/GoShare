using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities.Google;
using Application.Service;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class CalculateFeesForTripHandler : IRequestHandler<CalculateFeesForTripCommand, List<CartypeFeeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public CalculateFeesForTripHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<List<CartypeFeeDto>> Handle(CalculateFeesForTripCommand request, CancellationToken cancellationToken)
        {
            var carTypeFees = new List<CartypeFeeDto>();

            //var startLatitude = decimal.Parse(request.StartLatitude!);
            //var startLongitude = decimal.Parse(request.StartLongitude!);
            //var endLatitude = decimal.Parse(request.EndLatitude!);
            //var endLongitude = decimal.Parse(request.EndLongitude!);

            var origin = new Location
            {
                Latitude = request.StartLatitude,
                Longtitude = request.StartLongitude
            };

            var destination = new Location
            {
                Latitude = request.EndLatitude,
                Longtitude = request.EndLongitude
            };

            var distance = await GoogleMapsApiUtilities.ComputeDistanceMatrixAsync(origin, destination);

            var carTypes = await _unitOfWork.CartypeRepository.GetAllAsync();

            foreach (var carType in carTypes)
            {
                double price = await _unitOfWork.CartypeRepository.CalculatePriceForCarType(carType.Id, distance);

                carTypeFees.Add(new CartypeFeeDto
                {
                    CartypeId = carType.Id,
                    Capacity = carType.Capacity,
                    TotalPrice = price
                });
            }

            return carTypeFees;
        }
    }
}
