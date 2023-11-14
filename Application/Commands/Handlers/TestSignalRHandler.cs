using Application.SignalR;
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

        public TestSignalRHandler(IHubContext<SignalRHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<bool> Handle(TestSignalRCommand request, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.Group("testGroup").SendAsync("ReceiveMessage", "Test message");
            return true;
        }
    }
}
