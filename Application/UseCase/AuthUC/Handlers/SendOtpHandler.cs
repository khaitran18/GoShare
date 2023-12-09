using Application.Services;
using Application.Services.Interfaces;
using Application.UseCase.AuthUC.Commands;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuthUC.Handlers
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
            if (req.Status.Equals("pending")) return true;
            else return false;
        }
    }
}
