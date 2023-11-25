using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Common.Utilities.Google;
using Application.Common.Utilities.Google.Firebase;
using Application.Common.Utilities.SignalR;
using Application.Services.Interfaces;
using Application.SignalR;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<BackgroundServices> _logger;
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly ISettingService _settingService;
        private readonly IMapper _mapper;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public BackgroundServices(IUnitOfWork unitOfWork, ILogger<BackgroundServices> logger, IHubContext<SignalRHub> hubContext, ISettingService settingService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _hubContext = hubContext;
            _settingService = settingService;
            _mapper = mapper;
        }

        public async Task FindDriver(Guid tripId, Guid cartypeId)
        {
            _logger.LogInformation("Find driver started for tripId: {tripId}.", tripId);
            var trip = await _unitOfWork.TripRepository.GetByIdAsync(tripId);

            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), tripId);
            }

            var origin = trip.StartLocation;

            var radius = _settingService.GetSetting("FIND_DRIVER_RADIUS"); // km
            var maxRadius = _settingService.GetSetting("MAX_FIND_DRIVER_RADIUS"); // km
            var startTime = DateTimeUtilities.GetDateTimeVnNow();
            var timeout = TimeSpan.FromMinutes(_settingService.GetSetting("FIND_DRIVER_TIMEOUT")); // Set a timeout for finding a driver

            var driversToExclude = new List<User>();

            while (DateTimeUtilities.GetDateTimeVnNow() - startTime < timeout)
            {
                if (KeyValueStore.Instance.Get<string>($"CancelFindDriverTask_{trip.Id}") != null)
                {
                    _logger.LogInformation("Exitting find driver task for trip: {tripId}", tripId);
                    KeyValueStore.Instance.Remove($"CancelFindDriverTask_{trip.Id}");
                    return; // Exit early if cancellation was requested
                }
                var nearestDriver = new User();
                double shortestDistance = double.MaxValue;

                // Get all the available drivers within the current radius, excluding those in the exclusion list
                var drivers = (await _unitOfWork.UserRepository.GetActiveDriversWithinRadius(origin, radius))
                    .Where(driver => driver.Id != trip.PassengerId && driver.Id != trip.BookerId && !driversToExclude.Any(d => d.Id == driver.Id) &&
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

                    await semaphoreSlim.WaitAsync();

                    try
                    {
                        // Set the status of the nearest driver to BUSY for waiting response
                        nearestDriver.Status = UserStatus.BUSY;
                        nearestDriver.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                        await _unitOfWork.UserRepository.UpdateAsync(nearestDriver);

                        trip.DriverId = nearestDriver.Id;
                        trip.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                        await _unitOfWork.TripRepository.UpdateAsync(trip);

                        await _unitOfWork.Save();

                        KeyValueStore.Instance.Set($"TripConfirmationTask_{trip.Id}", "");

                        // Send a trip request to the driver
                        _logger.LogInformation("Found driver {driverName} for tripId: {tripId}. Waiting for driver's response...", nearestDriver.Name, trip.Id);
                        var accepted = await NotifyDriverAndAwaitResponse(nearestDriver, trip);

                        if (accepted)
                        {
                            await NotifyBackToPassenger(trip, nearestDriver);
                            KeyValueStore.Instance.Remove($"TripConfirmationTask_{trip.Id}");
                            _logger.LogInformation("Find driver completed for tripId: {tripId}.", tripId);
                            return; // Driver found, exit the loop
                        }
                        else
                        {
                            if (KeyValueStore.Instance.Get<string>($"CancelFindDriverTask_{trip.Id}") != null)
                            {
                                _logger.LogInformation("Exitting find driver task for trip: {tripId}", tripId);
                                KeyValueStore.Instance.Remove($"CancelFindDriverTask_{trip.Id}");
                                return; // Exit early if cancellation was requested
                            }
                            // Change status of driver back to active
                            nearestDriver.Status = UserStatus.ACTIVE;
                            nearestDriver.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                            await _unitOfWork.UserRepository.UpdateAsync(nearestDriver);

                            trip.DriverId = null;
                            await _unitOfWork.TripRepository.UpdateAsync(trip);

                            await _unitOfWork.Save();

                            _logger.LogInformation("Finding another driver...", tripId);

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
                if (radius < maxRadius && !drivers.Any(d => driversToExclude.Any(e => e.Id == d.Id)))
                {
                    _logger.LogInformation("No drivers found within {radius} km for tripId: {tripId}. Increasing search radius...", radius, trip.Id);
                    radius++;
                }

                // Wait for a short period of time before checking again
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            // If no driver was found, handle the timeout scenario
            _logger.LogWarning("No driver found for tripId: {tripId}. Handling timeout scenario...", tripId);
            await HandleTimeoutScenario(trip);
        }

        private async Task<bool> NotifyDriverAndAwaitResponse(User driver, Trip trip)
        {
            _logger.LogInformation("Notifying driver {driverName} about new trip request for tripId: {tripId}", driver.Name, trip.Id);

            if (!string.IsNullOrEmpty(driver.DeviceToken))
            {
                var result = await FirebaseUtilities.SendNotificationToDeviceAsync(driver.DeviceToken!,
                    "Yêu cầu chuyến mới",
                    "Bạn có yêu cầu chuyến xe mới",
                    new Dictionary<string, string>
                    {
                        { "tripId", trip.Id.ToString() }
                    });

                if (result == string.Empty)
                {
                    driver.DeviceToken = null;
                    await _unitOfWork.UserRepository.UpdateAsync(driver);
                    await _unitOfWork.Save();
                }
            }

            var groupName = SignalRUtilities.GetGroupNameForUser(driver, trip);

            await _hubContext.Clients.Group(groupName)
                .SendAsync("NotifyDriverNewTripRequest", _mapper.Map<TripDto>(trip));

            var timeout = TimeSpan.FromMinutes(_settingService.GetSetting("DRIVER_RESPONSE_TIMEOUT"));
            var startTime = DateTimeUtilities.GetDateTimeVnNow();

            var key = $"TripConfirmationTask_{trip.Id}";

            while (DateTimeUtilities.GetDateTimeVnNow() - startTime < timeout)
            {
                if (KeyValueStore.Instance.Get<string>($"CancelFindDriverTask_{trip.Id}") != null)
                {
                    _logger.LogInformation("Cancellation signal received for trip {tripId}", trip.Id);
                    return false; // Exit early if cancellation was requested
                }

                var status = KeyValueStore.Instance.Get<string>(key);

                if (status == "true")
                {
                    _logger.LogInformation("Driver {driverName} accepted the trip request for tripId: {tripId}", driver.Name, trip.Id);
                    return true;
                }
                else if (status == "false")
                {
                    _logger.LogInformation("Driver {driverName} refused the trip request for tripId: {tripId}", driver.Name, trip.Id);
                    return false;
                }

                // Wait for a short period of time before checking again
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            // Timeout
            _logger.LogWarning("Driver {driverName} did not respond to trip request for tripId: {tripId} within the timeout period.", driver.Name, trip.Id);
            return false;
        }

        private async Task NotifyBackToPassenger(Trip trip, User driver)
        {
            _logger.LogInformation("Notifying passenger about driver for tripId: {tripId}.", trip.Id);
            //var groupName = SignalRUtilities.GetGroupNameForUser(trip.Passenger, trip);

            if (!string.IsNullOrEmpty(trip.Passenger.DeviceToken))
            {
                var result = await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken,
                "Đặt chuyến thành công",
                $"Tài xế {driver.Name} đang trên đường đến đón bạn.",
                new Dictionary<string, string>
                {
                    { "tripId", trip.Id.ToString() }
                });

                if (result == string.Empty)
                {
                    trip.Passenger.DeviceToken = null;
                    await _unitOfWork.UserRepository.UpdateAsync(trip.Passenger);
                }
            }

            if (trip.Passenger.GuardianId != null && trip.Passenger.GuardianId == trip.BookerId)
            {
                if (!string.IsNullOrEmpty(trip.Passenger.Guardian!.DeviceToken))
                {
                    var result = await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.Guardian.DeviceToken,
                        "Đặt chuyến thành công",
                        $"Tài xế {driver.Name} đang trên đường đến đón người thân {trip.Passenger.Name} của bạn.",
                        new Dictionary<string, string>
                        {
                            { "tripId", trip.Id.ToString() }
                        });

                    if (result == string.Empty)
                    {
                        trip.Passenger.Guardian.DeviceToken = null;
                        await _unitOfWork.UserRepository.UpdateAsync(trip.Passenger.Guardian);
                    }
                }

                bool isSelfBooking = false;
                bool isNotificationForGuardian = true;
                await _hubContext.Clients.Group(trip.Passenger.GuardianId.ToString())
                    .SendAsync("NotifyPassengerDriverOnTheWay", _mapper.Map<UserDto>(driver), isSelfBooking, isNotificationForGuardian);

                isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerDriverOnTheWay", _mapper.Map<UserDto>(driver), isSelfBooking, isNotificationForGuardian);
            }
            else
            {
                bool isSelfBooking = true;
                bool isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerDriverOnTheWay", _mapper.Map<UserDto>(driver), isSelfBooking, isNotificationForGuardian);
            }

            await _unitOfWork.Save();
        }

        private async Task HandleTimeoutScenario(Trip trip)
        {
            _logger.LogInformation("Notifying passenger about trip timeout for tripId: {tripId}.", trip.Id);

            trip.Status = TripStatus.TIMEDOUT;
            await _unitOfWork.TripRepository.UpdateAsync(trip);

            // Change status of passenger back to inactive
            trip.Passenger.Status = UserStatus.INACTIVE;
            trip.Passenger.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
            await _unitOfWork.UserRepository.UpdateAsync(trip.Passenger);

            await _unitOfWork.Save();

            //var groupName = SignalRUtilities.GetGroupNameForUser(trip.Passenger, trip);

            if (!string.IsNullOrEmpty(trip.Passenger.DeviceToken))
            {
                var result = await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken,
                    "Hết thời gian chờ",
                    $"Chúng tôi thành thật xin lỗi, hiện tại chưa có tài xế phù hợp với bạn.",
                    new Dictionary<string, string>
                    {
                        { "tripId", trip.Id.ToString() }
                    });

                if (result == string.Empty)
                {
                    trip.Passenger.DeviceToken = null;
                    await _unitOfWork.UserRepository.UpdateAsync(trip.Passenger);
                }
            }

            if (trip.Passenger.GuardianId != null && trip.Passenger.GuardianId == trip.BookerId)
            {
                if (!string.IsNullOrEmpty(trip.Passenger.Guardian!.DeviceToken))
                {
                    var result = await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.Guardian.DeviceToken,
                        "Hết thời gian chờ",
                        $"Chúng tôi xin lỗi, hiện tại chưa có tài xế phù hợp với người thân {trip.Passenger.Name} của bạn.",
                        new Dictionary<string, string>
                        {
                            { "tripId", trip.Id.ToString() }
                        });

                    if (result == string.Empty)
                    {
                        trip.Passenger.Guardian.DeviceToken = null;
                        await _unitOfWork.UserRepository.UpdateAsync(trip.Passenger.Guardian);
                    }
                }

                bool isSelfBooking = false;
                bool isNotificationForGuardian = true;
                await _hubContext.Clients.Group(trip.Passenger.GuardianId.ToString())
                    .SendAsync("NotifyPassengerTripTimedOut", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);

                isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerTripTimedOut", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);
            }
            else
            {
                bool isSelfBooking = true;
                bool isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerTripTimedOut", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);
            }

            await _unitOfWork.Save();
        }

        public async Task ResetCancellationCountAndTime(Guid userId)
        {
            _logger.LogInformation("Resetting cancellation count and time for userId: {userId}", userId);
            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (user != null && user.CancellationBanUntil == null)
            {
                user.CanceledTripCount = 0;
                user.LastTripCancellationTime = null;
                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.Save();
            }
            else
            {
                _logger.LogWarning("User not found or user is currently banned for userId: {userId}. Skipping reset of cancellation count and time.", userId);
            }
        }

        public async Task CheckTransactionStatus(Guid TransactionId)
        {
            _logger.LogInformation("Checking transaction status: {TransactionId}", TransactionId);
            var transaction = await _unitOfWork.WallettransactionRepository.GetByIdAsync(TransactionId);
            if (transaction is not null)
            {
                if (transaction.Status.Equals(WalletTransactionStatus.PENDING))
                {
                    transaction.Status = WalletTransactionStatus.FAILED;
                    transaction.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    await _unitOfWork.Save();
                    _logger.LogWarning("Transaction: {TransactionId} Failed due to timeout", transaction.Id);
                }
                else _logger.LogInformation("Transaction: {TransactionId} is completed", TransactionId);
            }
            else _logger.LogWarning("Transaction: {TransactionId} is not found", TransactionId);
        }
    }
}