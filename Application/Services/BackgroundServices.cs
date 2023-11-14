using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Common.Utilities.Google;
using Application.Common.Utilities.Google.Firebase;
using Application.Services.Interfaces;
using Application.SignalR;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Hangfire;
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
        private readonly IMapper _mapper;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public BackgroundServices(IUnitOfWork unitOfWork, IMediator mediator, IHubContext<SignalRHub> hubContext, ISettingService settingService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _hubContext = hubContext;
            _settingService = settingService;
            _mapper = mapper;
        }

        public async Task FindDriver(Guid tripId, Guid cartypeId, CancellationToken cancellationToken)
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

            while (!cancellationToken.IsCancellationRequested && DateTime.Now - startTime < timeout)
            {
                var nearestDriver = new User();
                double shortestDistance = double.MaxValue;

                // Get all the available drivers within the current radius, excluding those in the exclusion list
                var drivers = (await _unitOfWork.UserRepository.GetActiveDriversWithinRadius(origin, radius))
                    .Where(driver => !driversToExclude.Any(d => d.Id == driver.Id) &&
                                    driver.Car != null && driver.Car.TypeId == cartypeId)
                    .ToList();

                if (drivers.Any())
                {
                    foreach (var driver in drivers)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return; // Exit early if cancellation was requested
                        }

                        var currentLocation = driver.Locations.FirstOrDefault(l => l.Type == LocationType.CURRENT_LOCATION);
                        if (currentLocation != null)
                        {
                            var distance = await GoogleMapsApiUtilities.ComputeDistanceMatrixAsync(origin, currentLocation);
                            if (distance < shortestDistance)
                            {
                                nearestDriver = await _unitOfWork.UserRepository.GetUserById(driver.Id.ToString());
                                shortestDistance = distance;
                            }
                        }
                    }

                    await semaphoreSlim.WaitAsync();

                    try
                    {
                        // Set the status of the nearest driver to BUSY for waiting response
                        nearestDriver.Status = UserStatus.BUSY;
                        nearestDriver.UpdatedTime = DateTime.Now;
                        await _unitOfWork.UserRepository.UpdateAsync(nearestDriver);

                        trip.DriverId = nearestDriver.Id;
                        trip.UpdatedTime = DateTime.Now;
                        await _unitOfWork.TripRepository.UpdateAsync(trip);

                        await _unitOfWork.Save();

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
                            // Change status of driver back to active
                            nearestDriver.Status = UserStatus.ACTIVE;
                            nearestDriver.UpdatedTime = DateTime.Now;
                            await _unitOfWork.UserRepository.UpdateAsync(nearestDriver);

                            trip.DriverId = null;
                            await _unitOfWork.TripRepository.UpdateAsync(trip);

                            await _unitOfWork.Save();

                            // Driver canceled or didn't respond, exclude the driver
                            driversToExclude.Add(nearestDriver);
                            KeyValueStore.Instance.Remove($"TripConfirmationTask_{trip.Id}");
                        }
                    }
                    finally
                    {
                        // Release the semaphore so that other threads can enter the critical section
                        semaphoreSlim.Release();
                    }
                }

                // No available drivers found within the current radius, increase it
                if (radius < maxRadius)
                {
                    radius++;
                }

                // Wait for a short period of time before checking again
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            // If no driver was found, handle the timeout scenario
            await HandleTimeoutScenario(trip);
        }

        private async Task<bool> NotifyDriverAndAwaitResponse(User driver, Trip trip)
        {
            //Console.WriteLine($"Found driver {driver.Id}!");

            //await FirebaseUtilities.SendNotificationToDeviceAsync(driver.DeviceToken!,
            //    "Yêu cầu chuyến mới",
            //    "Bạn có yêu cầu chuyến xe mới",
            //    new Dictionary<string, string>
            //    {
            //        { "tripId", trip.Id.ToString() }
            //    });

            await _hubContext.Clients.Group(driver.Id.ToString())
                .SendAsync("NotifyDriverNewTripRequest", _mapper.Map<TripDto>(trip));

            var timeout = TimeSpan.FromMinutes(_settingService.GetSetting("DRIVER_RESPONSE_TIMEOUT"));
            var startTime = DateTime.Now;

            var key = $"TripConfirmationTask_{trip.Id}";

            while (DateTime.Now - startTime < timeout)
            {
                var status = KeyValueStore.Instance.Get<string>(key);

                if (status == "true")
                {
                    return true;
                }
                else if (status == "false")
                {
                    return false;
                }

                // Wait for a short period of time before checking again
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            // Timeout
            return false;
        }

        private async Task NotifyBackToPassenger(Trip trip, User driver)
        {
            var groupName = GetGroupNameForUser(trip.Passenger, trip.Booker);

            //await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken!,
            //    "Đặt chuyến thành công",
            //    $"Tài xế {driver.Name} đang trên đường đến đón bạn.",
            //    new Dictionary<string, string>
            //    {
            //        { "tripId", trip.Id.ToString() }
            //    });

            if (trip.Passenger.GuardianId != null)
            {
                //await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.Guardian!.DeviceToken!,
                //    "Đặt chuyến thành công",
                //    $"Tài xế {driver.Name} đang trên đường đến đón người thân {trip.Passenger.Name} của bạn.",
                //    new Dictionary<string, string>
                //    {
                //        { "tripId", trip.Id.ToString() }
                //    });
            }

            await _hubContext.Clients.Group(groupName)
                .SendAsync("NotifyPassengerDriverOnTheWay", _mapper.Map<UserDto>(driver));
        }

        private async Task HandleTimeoutScenario(Trip trip)
        {
            trip.Status = TripStatus.TIMEDOUT;

            await _unitOfWork.TripRepository.UpdateAsync(trip);
            await _unitOfWork.Save();

            var groupName = GetGroupNameForUser(trip.Passenger, trip.Booker);

            //await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken!,
            //    "Hết thời gian chờ",
            //    $"Chúng tôi thành thật xin lỗi, hiện tại chưa có tài xế phù hợp với bạn.",
            //    new Dictionary<string, string>
            //    {
            //        { "tripId", trip.Id.ToString() }
            //    });

            await _hubContext.Groups.AddToGroupAsync(trip.PassengerId.ToString(), trip.Id.ToString());

            if (trip.Passenger.GuardianId != null)
            {
                //await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.Guardian!.DeviceToken!,
                //    "Hết thời gian chờ",
                //    $"Chúng tôi thành thật xin lỗi, hiện tại chưa có tài xế phù hợp với người thân của bạn.",
                //    new Dictionary<string, string>
                //    {
                //        { "tripId", trip.Id.ToString() }
                //    });
            }

            await _hubContext.Clients.Group(groupName)
                .SendAsync("NotifyPassengerTripTimedOut", trip);
        }

        private string GetGroupNameForUser(User user, User booker)
        {
            // If the user is a dependent and the booker is the guardian
            if (user.GuardianId != null && user.GuardianId == booker.Id)
            {
                return $"{user.Id}-{user.GuardianId}";
            }

            return $"{user.Id}";
        }

        public async Task ResetCancellationCountAndTime(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (user != null && user.CancellationBanUntil == null)
            {
                user.CanceledTripCount = 0;
                user.LastTripCancellationTime = null;
                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.Save();
            }
        }
    }
}