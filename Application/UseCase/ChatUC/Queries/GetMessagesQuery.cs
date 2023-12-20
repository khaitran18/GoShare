using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.ChatUC.Queries
{
    public class GetMessagesQuery : IRequest<List<ChatDto>>
    {
        public Guid TripId { get; set; } 
    }
}
