﻿using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.SignalR;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
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
            await _unitOfWork.ChatRepository.AddAsync(new Chat{
                Id = Guid.NewGuid(),
                Content = request.Content,
                Receiver = request.Receiver,
                Sender = (Guid)_claims.id!,
                Time = DateTimeUtilities.GetDateTimeVnNow()
            });
            await _unitOfWork.Save();
            return _hubContext.Clients.Group(request.Receiver.ToString()).SendAsync("ReceiveSMSMessages",request.Content);
        }
    }
}
