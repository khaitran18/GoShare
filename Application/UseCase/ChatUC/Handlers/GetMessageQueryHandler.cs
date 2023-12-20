﻿using Application.Common.Dtos;
using Application.SignalR;
using Application.UseCase.ChatUC.Queries;
using AutoMapper;
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
    public class GetMessageQueryHandler : IRequestHandler<GetMessagesQuery, List<ChatDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly UserClaims _claims;
        private readonly IMapper _mapper;

        public GetMessageQueryHandler(IUnitOfWork unitOfWork, IHubContext<SignalRHub> hubContext, UserClaims claims, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _claims = claims;
            _mapper = mapper;
        }

        public async Task<List<ChatDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            List<ChatDto> response = new List<ChatDto>();
            List<Chat> list = _unitOfWork.ChatRepository.GetChatByTripId(request.TripId);
            response = _mapper.Map<List<ChatDto>>(list);
//            await _hubContext.Clients.Group(_claims.id.ToString()).SendAsync("GetMessages", response);
            return response;
        }
    }
}
