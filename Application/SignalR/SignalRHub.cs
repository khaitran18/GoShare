using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.Common.Utilities.SignalR;
using Application.Services.Interfaces;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.SignalR
{
    [Authorize]
    public class SignalRHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ILogger<SignalRHub> _logger;

        public SignalRHub(IUnitOfWork unitOfWork, ITokenService tokenService, ILogger<SignalRHub> logger)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var token = httpContext.Request.Query["access_token"].ToString();
            var userId = _tokenService.GetGuid(token);

            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString()!);
            if (user != null)
            {
                var groupName = await GetGroupNameForUser(user);

                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                // Add to self-group
                await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
            }

            _logger.LogInformation("Client connected with ID: {userId}", userId.ToString());

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            var token = httpContext.Request.Query["access_token"].ToString();
            var userId = _tokenService.GetGuid(token);

            var user = await _unitOfWork.UserRepository.GetUserById(userId.ToString()!);
            if (user != null)
            {
                var groupName = await GetGroupNameForUser(user);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
            }

            _logger.LogInformation("Client disconnected with ID: {userId}", userId.ToString());

            await base.OnDisconnectedAsync(exception);
        }

        private async Task<string> GetGroupNameForUser(User user)
        {
            // If the user is a dependent
            if (user.GuardianId != null)
            {
                return $"{user.Id}-{user.GuardianId}";
            }
            // If the user is a guardian or driver and have dependents
            else
            {
                var dependents = await _unitOfWork.UserRepository.GetDependentsByGuardianId(user.Id);
                if (dependents.Any())
                {
                    foreach (var dependent in dependents)
                    {
                        var groupName = $"{dependent.Id}-{user.Id}";
                        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                    }
                }

                return $"{user.Id}";
            }
        }

        public async Task RequestLocation(string dependentId)
        {
            await Clients.Client(dependentId).SendAsync("RequestLocation");
        }

        public Task SendLocation(string dependentId, string location)
        {
            KeyValueStore.Instance.Set($"CurrentLocation_{dependentId}", location);
            return Task.CompletedTask;
        }

        public async Task SendDriverLocation(string driverLocation, string tripId)
        {
            var trip = await _unitOfWork.TripRepository.GetByIdAsync(Guid.Parse(tripId));

            if (trip == null)
            {
                return;
            }

            // Check the trip type
            if (trip.Type == TripType.SELF_BOOK || trip.Type == TripType.BOOK_FOR_DEP_NO_APP && trip.Status == TripStatus.GOING_TO_PICKUP)
            {
                // Only let user see driver location going to the pickup point
                await Clients.Group(trip.BookerId.ToString())
                    .SendAsync("UpdateDriverLocation", driverLocation);
            }
            else if (trip.Type == TripType.BOOK_FOR_DEP_WITH_APP)
            {
                // Only update driver location for guardian
                await Clients.Group(trip.BookerId.ToString())
                    .SendAsync("UpdateDriverLocation", driverLocation);
            }
        }

        public async Task SendDependentLocation(string dependentLocation, string tripId)
        {
            var trip = await _unitOfWork.TripRepository.GetByIdAsync(Guid.Parse(tripId));

            if (trip == null)
            {
                return;
            }

            // Check the trip type
            if (trip.Type == TripType.BOOK_FOR_DEP_WITH_APP)
            {
                // Update dependent location for driver
                if (trip.Status == TripStatus.GOING_TO_PICKUP)
                {
                    await Clients.Group(trip.DriverId.ToString())
                        .SendAsync("UpdateDependentLocation", dependentLocation);
                }

                await Clients.Group(trip.BookerId.ToString())
                        .SendAsync("UpdateDependentLocation", dependentLocation);
            }
        }
    }
}
