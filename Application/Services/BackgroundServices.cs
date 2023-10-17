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

            var driversToExclude = new List<User>();

            while (DateTime.Now - startTime < timeout)
            {
                var nearestDriver = new User();
                double shortestDistance = double.MaxValue;
                // Get all the available drivers within the current radius, excluding those in the exclusion list
                var drivers = (await _unitOfWork.UserRepository.GetActiveDriversWithinRadius(origin, radius))
                    .Where(driver => !driversToExclude.Contains(driver))
                    .ToList();

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

                    var tripConfirmationTask = new TaskCompletionSource<bool>();
                    tripConfirmationTasks.Add(tripId, tripConfirmationTask);

                    // Send a trip request to the driver
                    var accepted = await NotifyDriverAndAwaitResponse(nearestDriver, trip);

                    if (accepted)
                    {
                        await NotifyBackToPassenger(trip, nearestDriver);
                        return; // Driver found, exit the loop
                    }
                    else
                    {
                        // Driver canceled or didn't respond, exclude the driver
                        driversToExclude.Add(nearestDriver);
                        tripConfirmationTasks.Remove(tripId);
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
            var tripConfirmationTask = tripConfirmationTasks[trip.Id];

            string content = (trip.StartLocation.Address == null || trip.EndLocation.Address == null)
                ? "Bạn có yêu cầu chuyến xe mới"
                : $"Bạn có muốn đón khách từ {trip.StartLocation.Address} đi {trip.EndLocation.Address} không?";

            await FirebaseUtilities.SendNotificationToDeviceAsync(driver.DeviceToken!,
                "Yêu cầu chuyến mới",
                content,
                new Dictionary<string, string>
                {
                    { "tripId", trip.Id.ToString() }
                });

            // Wait for the driver's response with a two-minute timeout
            var completedTask = await Task.WhenAny(tripConfirmationTask.Task, Task.Delay(TimeSpan.FromMinutes(2)));

            if (completedTask == tripConfirmationTask.Task)
            {
                return tripConfirmationTask.Task.Result;
            }
            else
            {
                return false;
            }
        }

        private async Task NotifyBackToPassenger(Trip trip, User driver)
        {
            await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken!,
                "Đặt chuyến thành công",
                $"Tài xế {driver.Name} đang trên đường đến chỗ bạn.",
                new Dictionary<string, string>
                {
                    { "tripId", trip.Id.ToString() }
                });
        }

        private async Task HandleTimeoutScenario(Trip trip)
        {
            trip.Status = TripStatus.TIMEDOUT;

            await _unitOfWork.TripRepository.UpdateAsync(trip);

            await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken!,
                "Hết thời gian chờ",
                $"Chúng tôi thành thật xin lỗi, hiện tại chưa có tài xế phù hợp với bạn.",
                new Dictionary<string, string>
                {
                    { "tripId", trip.Id.ToString() }
                });
        }
    }
}
