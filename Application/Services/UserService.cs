﻿using Application.Common.Dtos;
using Application.Services.Interfaces;
using Domain.DataModels;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        public Task<bool> CheckDependentStatus(IUnitOfWork unitOfWork, Guid UserId, Guid GuadianId)
        {
            var list = unitOfWork.UserRepository.GetDependentsByGuardianId(GuadianId).Result;
            return Task.FromResult(list.Any(u => u.Id.Equals(UserId))) ;
        }

        public async Task<List<DependentTripInfo>?> GetCurrentDenpendentTrips(IUnitOfWork unitOfWork, Guid UserId)
        {
            var result = new List<DependentTripInfo>();
            var list = await unitOfWork.UserRepository.GetDependentsByGuardianId(UserId);
            foreach (var dependent in list)
            {
                Trip? t = await unitOfWork.TripRepository.GetOngoingTripByPassengerId(dependent.Id);
                if (t is not null)
                {
                    DependentTripInfo d = new DependentTripInfo();
                    d.Id = t.Id;
                    d.Name = dependent.Name;
                    d.DependentId = (Guid)t.PassengerId!;
                    result.Add(d);
                }
            }
            //check trip type 2
            List<Trip> trips = await unitOfWork.TripRepository.GetOnGoingTripBookForDepWithNoPhone(UserId);
            foreach (var trip in trips)
            {
                DependentTripInfo d = new DependentTripInfo();
                d.Id = trip.Id;
                d.Name = trip.PassengerName;
                d.DependentId = trip.PassengerId;
                result.Add(d);
            }
            return result;
        }
    }
}
