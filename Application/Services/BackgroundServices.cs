using Application.Common.Exceptions;
using Domain.DataModels;
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

        public BackgroundServices(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        //public async void FindDriver(Guid tripId)
        //{
        //    var trip = await _unitOfWork.TripRepository.GetByIdAsync(tripId);

        //    if (trip == null)
        //    {
        //        throw new NotFoundException(nameof(Trip), tripId);
        //    }

        //    var origin = await trip.StartLocation;

        //    var radius = 1;
        //    var step = 1;
        //    var maxRadius = 5;
        //    var startTime = DateTime.Now;

        //    // Loop until a driver is found or the timeout is reached
        //    while (true)
        //    {
        //        // Get all the drivers from the database using the repository pattern
        //        var drivers = await _unitOfWork.UserRepository.GetAllAsync(u => u.Role == "Driver");

        //        // Filter the drivers by their availability and location within the radius
        //        drivers = drivers.Where(d => d.Location != null && IsWithinRadius(origin, d.Location, radius));

        //        // If no drivers are found, increase the radius and continue the loop
        //        if (!drivers.Any())
        //        {
        //            radius += step;
        //            if (radius > maxRadius)
        //            {
        //                radius = maxRadius;
        //            }
        //            continue;
        //        }

        //        // Sort the drivers by their distance to the origin using Google Matrix API
        //        drivers = drivers.OrderBy(d => GetDistance(origin, d.Location));

        //        // Loop through the drivers and send them a notification using Firebase Cloud Messaging
        //        foreach (var driver in drivers)
        //        {
        //            var notification = new Notification
        //            {
        //                Title = "New trip request",
        //                Body = $"Do you want to accept a trip from {origin} to {trip.Destination}?",
        //                Data = new Dictionary<string, string>
        //            {
        //                { "tripId", trip.Id.ToString() },
        //                { "action", "accept" }
        //            }
        //            };

        //            await SendNotificationAsync(driver.DeviceToken, notification);

        //            // Wait for the driver response time and check if the driver accepted the trip
        //            await Task.Delay(TimeSpan.FromMinutes(DriverResponseTime));
        //            var updatedTrip = await _unitOfWork.Trips.GetByIdAsync(trip.Id);

        //            if (updatedTrip.Status == "Going" && updatedTrip.Driver.Id == driver.Id)
        //            {
        //                // The driver accepted the trip, send a notification to the passenger and return
        //                var passengerNotification = new Notification
        //                {
        //                    Title = "Your trip is confirmed",
        //                    Body = $"Your driver is {driver.Name} and he/she is on the way.",
        //                    Data = new Dictionary<string, string>
        //                {
        //                    { "tripId", trip.Id.ToString() },
        //                    { "action", "view" }
        //                }
        //                };

        //                await SendNotificationAsync(trip.Passenger.DeviceToken, passengerNotification);
        //                return;
        //            }
        //        }

        //        // If none of the drivers accepted the trip, check if the timeout is reached
        //        var currentTime = DateTime.Now;
        //        var elapsedTime = currentTime - startTime;

        //        if (elapsedTime.TotalMinutes >= Timeout)
        //        {
        //            // The timeout is reached, update the trip status to timed out and send a notification to the passenger
        //            await _mediator.Send(new UpdateTripCommand(trip.Id, null, "TimedOut"));

        //            var passengerNotification = new Notification
        //            {
        //                Title = "Your trip request has timed out",
        //                Body = $"We are sorry, we could not find any driver for your trip.",
        //                Data = new Dictionary<string, string>
        //            {
        //                { "tripId", trip.Id.ToString() },
        //                { "action", "cancel" }
        //            }
        //            };

        //            await SendNotificationAsync(trip.Passenger.DeviceToken, passengerNotification);
        //            return;
        //        }
        //    }
        //}

        //private bool IsWithinRadius(Location origin, Location destination, double radius)
        //{
        //    // Calculate the distance between two locations using Haversine formula
        //    var earthRadius = 6371.0; // Earth radius in km
        //    var lat1 = origin.Latitude * Math.PI / 180.0; // Convert degrees to radians
        //    var lon1 = origin.Longitude * Math.PI / 180.0;
        //    var lat2 = destination.Latitude * Math.PI / 180.0;
        //    var lon2 = destination.Longitude * Math.PI / 180.0;
        //    var dLat = lat2 - lat1;
        //    var dLon = lon2 - lon1;

        //    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
        //            Math.Cos(lat1) * Math.Cos(lat2) *
        //            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        //    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        //    var distance = earthRadius * c; // Distance in km

        //    // Check if the distance is less than or equal to the radius
        //    return distance <= radius;
        //}

        public async Task FindDriver(Guid tripId)
        {
            var trip = await _unitOfWork.TripRepository.GetByIdAsync(tripId);

            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), tripId);
            }

            var origin = await trip.StartLocation;

            var radius = 1;
            var maxRadius = 5;
            var startTime = DateTime.Now;
            var timeout = TimeSpan.FromMinutes(5); // Set a timeout for finding a driver

            while (DateTime.Now - startTime < timeout)
            {
                var nearestDriver = new User();
                double shortestDistance = double.MaxValue;
                // Get all the available drivers within the current radius
                var drivers = await _unitOfWork.UserRepository.GetDriversWithinRadius(origin, radius);

                if (drivers.Any())
                {
                    foreach (var driver in drivers)
                    {
                        var distance = await GoogleMapsApiUtilities.ComputeDistanceMatrixAsync(origin, driver.Location);

                        // If this driver is nearer, update nearestDriver and shortestDistance
                        if (distance < shortestDistance)
                        {
                            nearestDriver = driver;
                            shortestDistance = distance;
                        }
                    }
                    // Sort the drivers by distance using Google Matrix API
                    drivers = drivers.OrderBy(d => await GoogleMapsApiUtilities.ComputeDistanceMatrixAsync(origin, d.Location));

                    foreach (var driver in drivers)
                    {
                        // Send a trip request to the driver
                        var accepted = await NotifyDriverAndAwaitResponse(driver, trip);

                        if (accepted)
                        {
                            // The driver accepted the trip
                            await NotifyPassengerAndFinishTripSetup(trip, driver);
                            return;
                        }
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
            var notification = new Notification
            {
                Title = "New trip request",
                Body = $"Do you want to accept a trip from {trip.StartLocation} to {trip.Destination}?",
                Data = new Dictionary<string, string>
        {
            { "tripId", trip.Id.ToString() },
            { "action", "accept" }
        }
            };

            await SendNotificationAsync(driver.DeviceToken, notification);

            await Task.Delay(TimeSpan.FromMinutes(DriverResponseTime));
            var updatedTrip = await _unitOfWork.TripRepository.GetByIdAsync(trip.Id);

            return updatedTrip.Status == "Going" && updatedTrip.Driver.Id == driver.Id;
        }

        private async Task NotifyPassengerAndFinishTripSetup(Trip trip, User driver)
        {
            // Update the trip status, driver information, and price
            trip.Status = "Going";
            trip.Driver = driver;
            trip.Price = CalculatePrice(trip); // Implement your price calculation logic

            // Notify the passenger
            var passengerNotification = new Notification
            {
                Title = "Your trip is confirmed",
                Body = $"Your driver is {driver.Name} and they are on the way.",
                Data = new Dictionary<string, string>
        {
            { "tripId", trip.Id.ToString() },
            { "action", "view" }
        }
            };

            await SendNotificationAsync(trip.Passenger.DeviceToken, passengerNotification);

            // Update the trip in the database
            await _unitOfWork.TripRepository.UpdateAsync(trip);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task HandleTimeoutScenario(Trip trip)
        {
            // Update the trip status to timed out
            await _mediator.Send(new UpdateTripCommand(trip.Id, null, "TimedOut"));

            // Notify the passenger about the timeout
            var passengerNotification = new Notification
            {
                Title = "Your trip request has timed out",
                Body = $"We are sorry, we could not find any driver for your trip.",
                Data = new Dictionary<string, string>
        {
            { "tripId", trip.Id.ToString() },
            { "action", "cancel" }
        }
            };

            await SendNotificationAsync(trip.Passenger.DeviceToken, passengerNotification);
        }

        private decimal CalculatePrice(Trip trip)
        {
            // Implement your price calculation logic here
            // You can calculate the price based on distance, time, or any other factors
            // and return the calculated price as a decimal.
            // Example: return 15.00M;
        }

    }
}
