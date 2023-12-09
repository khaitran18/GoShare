using Application.Common.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreateVnpayTopupRequest(UserClaims user, double amount, Guid TransactionId);
        VnpayCallbackResponse PaymentExecute(IQueryCollection collection);
    }
}
