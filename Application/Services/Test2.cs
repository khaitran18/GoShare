using Domain.Enumerations;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CreateTripCommand : IRequest<Trip>
    {
        public string Destination { get; set; }
        public int NumberOfPeople { get; set; }
    }

    public class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, Trip>
    {
        private readonly ITripRepository _tripRepository;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IMediator _mediator;

        public CreateTripCommandHandler(ITripRepository tripRepository, IBackgroundJobClient backgroundJobs, IMediator mediator)
        {
            _tripRepository = tripRepository;
            _backgroundJobs = backgroundJobs;
            _mediator = mediator;
        }

        public async Task<Trip> Handle(CreateTripCommand request, CancellationToken cancellationToken)
        {
            var trip = new Trip
            {
                Destination = request.Destination,
                NumberOfPeople = request.NumberOfPeople,
                Status = TripStatus.Pending
            };

            await _tripRepository.AddAsync(trip);

            // Schedule a background job to find the nearest driver
            _backgroundJobs.Enqueue(() => FindNearestDriver(trip.Id));

            return trip;
        }

        [AutomaticRetry(Attempts = 0)]
        [JobDisplayName("Find nearest driver for trip #{0}")]
        public async Task FindNearestDriver(int tripId)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null || trip.Status != TripStatus.PENDING)
                return;

            var driver = await _mediator.Send(new FindNearestDriverQuery(trip.StartPoint));
            if (driver == null)
            {
                // If no driver found within 5km or after 30 minutes, cancel the trip
                trip.Status = TripStatus.TimedOut;
                await _tripRepository.UpdateAsync(trip);
                return;
            }

            // Send a notification to the driver
            await _mediator.Send(new SendNotificationCommand(driver.Id, $"New trip request: {trip.Id}"));

            // If the driver does not respond within 2 minutes, find the next nearest driver
            _backgroundJobs.Schedule(() => FindNearestDriver(trip.Id), TimeSpan.FromMinutes(2));
        }
    }

    public class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, int>
    {
        private readonly ITripRepository _tripRepository;
        private readonly IHangfireService _hangfireService;

        public CreateTripCommandHandler(ITripRepository tripRepository, IHangfireService hangfireService)
        {
            _tripRepository = tripRepository;
            _hangfireService = hangfireService;
        }

        public async Task<int> Handle(CreateTripCommand request, CancellationToken cancellationToken)
        {
            // create a new trip entity with pending status and current time
            var trip = new Trip
            {
                Destination = request.Destination,
                NumberOfPeople = request.NumberOfPeople,
                Status = "pending",
                CreatedAt = DateTime.Now
            };

            // save the trip to the database and get its id
            var tripId = await _tripRepository.AddAsync(trip);

            // schedule a background job to find a driver for the trip using hangfire service
            _hangfireService.ScheduleFindDriverJob(tripId);

            // return the trip id
            return tripId;
        }
    }

    public interface IHangfireService
    {
        void ScheduleFindDriverJob(int tripId);
    }

    public class HangfireService : IHangfireService
    {
        private readonly IMediator _mediator;

        public HangfireService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void ScheduleFindDriverJob(int tripId)
        {
            // create a background job that runs after 10 seconds and calls the FindDriver method with the trip id
            BackgroundJob.Schedule(() => FindDriver(tripId), TimeSpan.FromSeconds(10));
        }

        public async void FindDriver(int tripId)
        {
            // get the trip details from the database using a query
            var trip = await _mediator.Send(new GetTripByIdQuery { Id = tripId });

            // check if the trip is still pending and not timed out
            if (trip != null && trip.Status == "pending" && DateTime.Now - trip.CreatedAt < TimeSpan.FromMinutes(30))
            {
                // get the nearest driver to the trip destination using a query
                var driver = await _mediator.Send(new GetNearestDriverQuery { Destination = trip.Destination });

                // check if there is a driver available
                if (driver != null)
                {
                    // send a notification to the driver using firebase cloud messaging service
                    var notificationResult = await _mediator.Send(new SendNotificationCommand { DriverId = driver.Id, TripId = trip.Id });

                    // check if the notification was sent successfully
                    if (notificationResult)
                    {
                        // schedule a background job that runs after 2 minutes and calls the CheckDriverResponse method with the driver and trip ids
                        BackgroundJob.Schedule(() => CheckDriverResponse(driver.Id, trip.Id), TimeSpan.FromMinutes(2));
                    }
                    else
                    {
                        // retry finding another driver for the trip
                        FindDriver(tripId);
                    }
                }
                else
                {
                    // retry finding a driver for the trip within 5 km radius using routes api service
                    var retryResult = await _mediator.Send(new RetryFindDriverCommand { TripId = tripId });

                    // check if there is a driver available within 5 km radius
                    if (retryResult)
                    {
                        // schedule a background job that runs after 2 minutes and calls the CheckDriverResponse method with the driver and trip ids
                        BackgroundJob.Schedule(() => CheckDriverResponse(retryResult.DriverId, retryResult.TripId), TimeSpan.FromMinutes(2));
                    }
                    else
                    {
                        // no driver available, cancel the trip and notify the user
                        await _mediator.Send(new CancelTripCommand { TripId = tripId, Reason = "No driver available" });
                        await _mediator.Send(new NotifyUserCommand { TripId = tripId, Message = "Sorry, we could not find any driver for your trip. Please try again later." });
                    }
                }
            }
        }

        public async void CheckDriverResponse(int driverId, int tripId)
        { // get the driver response from the database using a query var response = await _mediator.Send(new GetDriverResponseQuery { DriverId = driverId, TripId = tripId });

            // check if the driver accepted or rejected or did not respond to the trip request
            if (response == "accepted")
            {
                // update the trip status as going and assign the driver id to it using a command
                await _mediator.Send(new UpdateTripCommand { Id = tripId, Status = "going", DriverId = driverId });

                // notify the user that the driver has accepted the trip and send the driver details using a command
                await _mediator.Send(new NotifyUserCommand
                {
                    TripId = tripId,
                    Message = $"Your driver {driver.Name} has accepted your trip. He will arrive at your location in {driver.EstimatedTime} minutes. You can track his location on the map."
                });
            }
            else if (response == "rejected" || response == null)
            {
                // find another driver for the trip
                FindDriver(tripId);
            }
        }
        // Calculate the distance between two locations using Google Matrix API
        private async Task<double> GetDistance(Location origin, Location destination)
        {
            // Use Google Matrix API to get the distance in meters
            var originString = $"{origin.Latitude},{origin.Longitude}";
            var destinationString = $"{destination.Latitude},{destination.Longitude}";
            var apiKey = "YOUR_API_KEY"; // Replace with your own API key
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={originString}&destinations={destinationString}&key={apiKey}";

            // Send a GET request to the API and parse the response
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var distance = json["rows"][0]["elements"][0]["distance"]["value"].Value<double>();
                return distance / 1000.0; // Convert meters to km
            }
        }

        // Send a notification to a device using Firebase Cloud Messaging
        private async Task SendNotificationAsync(string deviceToken, Notification notification)
        {
            // Use Firebase Cloud Messaging API to send a notification
            var apiKey = "YOUR_API_KEY"; // Replace with your own API key
            var url = "https://fcm.googleapis.com/fcm/send";

            // Create the message payload
            var message = new
            {
                to = deviceToken,
                notification = notification,
                data = notification.Data
            };

            // Serialize the message to JSON
            var json = JsonConvert.SerializeObject(message);

            // Send a POST request to the API with the JSON payload and the API key
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={apiKey}");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var response = await client.PostAsync(url, new StringContent(json));
                var content = await response.Content.ReadAsStringAsync();
                // Handle the response as needed
            }
        }
    }
}