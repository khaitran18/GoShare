using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities.Google;
using Application.Common.Utilities.Google.Firebase;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BackgroundServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private Dictionary<Guid, TaskCompletionSource<bool>> tripConfirmationTasks = new Dictionary<Guid, TaskCompletionSource<bool>>();

        public BackgroundServices(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task FindDriver(Guid tripId)
        {
            var trip = await _unitOfWork.TripRepository.GetByIdAsync(tripId);

            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), tripId);
            }

            var origin = trip.StartLocation;

            var radius = 1.0; //km
            var maxRadius = 5.0; //km
            var startTime = DateTime.Now;
            var timeout = TimeSpan.FromMinutes(5); // Set a timeout for finding a driver

            var tripConfirmationTask = new TaskCompletionSource<bool>();
            tripConfirmationTasks.Add(tripId, tripConfirmationTask);

            while (DateTime.Now - startTime < timeout)
            {
                var nearestDriver = new User();
                double shortestDistance = double.MaxValue;
                // Get all the available drivers within the current radius
                var drivers = await _unitOfWork.UserRepository.GetActiveDriversWithinRadius(origin, radius); //Haversine formula

                if (drivers.Any())
                {
                    foreach (var driver in drivers)
                    {
                        var currentLocation = driver.Locations.FirstOrDefault(l => l.Type == LocationType.CURRENT_LOCATION);
                        if (currentLocation != null)
                        {
                            var distance = await GoogleMapsApiUtilities.ComputeDistanceMatrixAsync(origin, currentLocation);
                            if (distance < shortestDistance)
                            {
                                nearestDriver = driver;
                                shortestDistance = distance;
                            }
                        }
                    }

                    // Send a trip request to the driver
                    var accepted = await NotifyDriverAndAwaitResponse(nearestDriver, trip);

                    if (accepted)
                    {
                        tripConfirmationTask.SetResult(true);
                        tripConfirmationTasks.Remove(tripId); // Remove the task from the dictionary
                        return;
                    }
                }

                // No available drivers found within the current radius, increase it
                if (radius < maxRadius)
                {
                    radius++;
                }
            }

            // If no driver was found, handle the timeout scenario
            await HandleTimeoutScenario(trip);
        }

        private async Task<bool> NotifyDriverAndAwaitResponse(User driver, Trip trip)
        {
            await FirebaseUtilities.SendNotificationToDeviceAsync(driver.DeviceToken!,
                "New trip request",
                $"Do you want to accept a trip from { trip.StartLocation.Address} to { trip.EndLocation.Address}?",
                new Dictionary<string, string> 
                { 
                    { "tripId", trip.Id.ToString() }
                });

            await Task.Delay(TimeSpan.FromMinutes(2));
            var updatedTrip = await _unitOfWork.TripRepository.GetByIdAsync(trip.Id);

            return updatedTrip.Status == TripStatus.GOING && updatedTrip.Driver.Id == driver.Id;
        }

        private async Task NotifyPassengerAndFinishTripSetup(Trip trip, User driver)
        {
            trip.Status = TripStatus.GOING;
            trip.DriverId = driver.Id; 
            //trip.Price = CalculatePrice(trip);

            await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken!,
                "Your trip is confirmed",
                $"Your driver is {driver.Name} and they are on the way.",
                new Dictionary<string, string>
                {
                    { "tripId", trip.Id.ToString() }
                });

            await _unitOfWork.TripRepository.UpdateAsync(trip);
        }

        private async Task HandleTimeoutScenario(Trip trip)
        {
            trip.Status = TripStatus.TIMEDOUT;

            await _unitOfWork.TripRepository.UpdateAsync(trip);

            await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken!,
                "Your trip request has timed out",
                $"We are sorry, we could not find any driver for your trip.",
                new Dictionary<string, string>
                {
                    { "tripId", trip.Id.ToString() }
                });
        }

        //private decimal CalculatePrice(Trip trip)
        //{
            
        //}

    }
}
