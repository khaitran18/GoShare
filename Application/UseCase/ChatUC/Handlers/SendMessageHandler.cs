using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.SignalR;
using Application.UseCase.ChatUC.Commands;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.ChatUC.Handlers
{
    public class SendMessageHandler : IRequestHandler<SendMessageCommand, Task>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly UserClaims _claims;

        public SendMessageHandler(IUnitOfWork unitOfWork, IHubContext<SignalRHub> hubContext, UserClaims claims)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _claims = claims;
        }

        public async Task<Task> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var t = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);
            if (t is null) throw new BadRequestException("Trip for chat is not found");
            else
            {
                await _unitOfWork.ChatRepository.AddAsync(new Chat
                {
                    Id = Guid.NewGuid(),
                    Content = request.Content,
                    Receiver = request.Receiver,
                    Sender = (Guid)_claims.id!,
                    TripId = t.Id,
                    Time = DateTimeUtilities.GetDateTimeVnNow()
                });
                await _unitOfWork.Save();
                return _hubContext.Clients.Group(request.Receiver.ToString()).SendAsync("ReceiveSMSMessages", request.Content);
            }
        }
    }
}
