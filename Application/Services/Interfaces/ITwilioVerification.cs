using Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Verify.V2.Service;

namespace Application.Services.Interfaces
{
    public interface ITwilioVerification
    {
        Task<VerificationResource> StartVerificationAsync(string phoneNumber, string channel);

        Task<bool> CheckVerificationAsync(string phoneNumber, string code);
        Task<DateTime> GenerateOtpExpiryTime();
    }
}
