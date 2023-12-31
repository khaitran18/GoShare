﻿using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities.Google;
using Application.Services.Interfaces;
using Application.UseCase.TripUC.Commands;
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
    public class CalculateFeesForTripHandler : IRequestHandler<CalculateFeesForTripCommand, List<CartypeFeeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CalculateFeesForTripHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CartypeFeeDto>> Handle(CalculateFeesForTripCommand request, CancellationToken cancellationToken)
        {
            var carTypeFees = new List<CartypeFeeDto>();

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

            var carTypes = await _unitOfWork.CartypeRepository.GetAllCartypeAsync();

            foreach (var carType in carTypes)
            {
                var fee = carType.Fees.FirstOrDefault();
                if (fee != null)
                {
                    double price = await _unitOfWork.CartypeRepository.CalculatePriceForCarType(carType.Id, distance);

                    carTypeFees.Add(new CartypeFeeDto
                    {
                        CartypeId = carType.Id,
                        Capacity = carType.Capacity,
                        TotalPrice = price,
                        Image = carType.Image
                    });
                }
            }

            return carTypeFees;
        }
    }
}
