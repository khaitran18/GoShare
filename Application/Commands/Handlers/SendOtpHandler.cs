using Application.Services;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class SendOtpHandler : IRequestHandler<SendOtpCommand, bool>
    {
        private readonly ITwilioVerification _verification;

        public SendOtpHandler(ITwilioVerification verification)
        {
            _verification = verification;
        }

        public async Task<bool> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            var req = await _verification.StartVerificationAsync(request.phone!, "sms");
            return req.IsValid;
        }
    }
}
