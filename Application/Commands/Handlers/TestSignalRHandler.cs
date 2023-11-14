using Application.SignalR;
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
    public class TestSignalRHandler : IRequestHandler<TestSignalRCommand, bool>
    {
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;

        public TestSignalRHandler(IHubContext<SignalRHub> hubContext, IUnitOfWork unitOfWork)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(TestSignalRCommand request, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.Group(request.UserId.ToString())
                .SendAsync("ReceiveMessage", "Test succeeded!");
            return true;
        }
    }
}
