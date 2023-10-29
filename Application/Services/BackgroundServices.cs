using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Common.Utilities.Google;
using Application.Common.Utilities.Google.Firebase;
using Application.Services.Interfaces;
using Application.SignalR;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly ISettingService _settingService;

        public BackgroundServices(IUnitOfWork unitOfWork, IMediator mediator, IHubContext<SignalRHub> hubContext, ISettingService settingService)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _hubContext = hubContext;
            _settingService = settingService;
        }

        public async Task FindDriver(Guid tripId, Guid cartypeId)
        {
            var trip = await _unitOfWork.TripRepository.GetByIdAsync(tripId);

            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), tripId);
            }

            var origin = trip.StartLocation;

            var radius = _settingService.GetSetting("FIND_DRIVER_RADIUS"); // km
            var maxRadius = _settingService.GetSetting("MAX_FIND_DRIVER_RADIUS"); // km
            var startTime = DateTime.Now;
            var timeout = TimeSpan.FromMinutes(_settingService.GetSetting("FIND_DRIVER_TIMEOUT")); // Set a timeout for finding a driver

            var driversToExclude = new List<User>();

            while (DateTime.Now - startTime < timeout)
            {
                var nearestDriver = new User();
                double shortestDistance = double.MaxValue;

                // Get all the available drivers within the current radius, excluding those in the exclusion list
                var drivers = (await _unitOfWork.UserRepository.GetActiveDriversWithinRadius(origin, radius))
                    .Where(driver => !driversToExclude.Contains(driver) &&
                                    driver.Car != null && driver.Car.TypeId == cartypeId)
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

                    KeyValueStore.Instance.Set($"TripConfirmationTask_{trip.Id}", "");

                    // Send a trip request to the driver
                    var accepted = await NotifyDriverAndAwaitResponse(nearestDriver, trip);

                    if (accepted)
                    {
                        await NotifyBackToPassenger(trip, nearestDriver);
                        KeyValueStore.Instance.Remove($"TripConfirmationTask_{trip.Id}");
                        return; // Driver found, exit the loop
                    }
                    else
                    {
                        // Driver canceled or didn't respond, exclude the driver
                        driversToExclude.Add(nearestDriver);
                        KeyValueStore.Instance.Remove($"TripConfirmationTask_{trip.Id}");
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

            await _hubContext.Clients.User(driver.Id.ToString())
                .SendAsync("NotifyDriverNewTripRequest", content);

            // Set a two-minute timeout for driver response
            var timeoutTask = Task.Delay(TimeSpan.FromMinutes(_settingService.GetSetting("DRIVER_RESPONSE_TIMEOUT")));

            var key = $"TripConfirmationTask_{trip.Id}";
            var status = KeyValueStore.Instance.Get<string>(key);

            // Wait for either driver response or timeout
            var completedTask = await Task.WhenAny(Task.FromResult(status), timeoutTask);

            if (completedTask == timeoutTask)
            {
                return false;
            }
            else if (status == "true")
            {
                return true;
            }
            else if (status == "false")
            {
                return false;
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
            await _hubContext.Clients.User(trip.PassengerId.ToString())
                .SendAsync("NotifyPassengerDriverOnTheWay", $"Tài xế {driver.Name} đang trên đường đến chỗ bạn.");
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

            await _hubContext.Clients.User(trip.PassengerId.ToString())
                .SendAsync("NotifyPassengerTripTimedOut", "Chúng tôi thành thật xin lỗi, hiện tại chưa có tài xế phù hợp với bạn.");
        }
    }
}
