using Application.Commands;
using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.Configuration;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly VnpayConfig _config;

        public PaymentService(VnpayConfig config)
        {
            _config = config;
        }

        public Task<string> CreateVnpayTopupRequest(UserClaims user, double amount, Guid TransactionId)
        {
            string command = "pay";
            var pay = new VnPayLibrary();
            pay.AddRequestData("vnp_Version", _config.Version);
            pay.AddRequestData("vnp_Command", command);
            pay.AddRequestData("vnp_TmnCode", _config.TmnCode);
            pay.AddRequestData("vnp_Amount", ((int)amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", DateTimeUtilities.GetDateTimeVnNow().ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _config.CurrCode);
            pay.AddRequestData("vnp_IpAddr", user.UserIp);
            pay.AddRequestData("vnp_Locale", _config.Locale);
            pay.AddRequestData("vnp_OrderInfo", $"Topup_{TransactionId}_{user.id}_{amount}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", _config.CallBackUrl);
            pay.AddRequestData("vnp_TxnRef", TransactionId.ToString());
            var paymentUrl =
                pay.CreateRequestUrl(_config.BaseUrl, _config.HashSecret);
            return Task.FromResult(paymentUrl);
        }

        public VnpayCallbackResponse PaymentExecute(IQueryCollection collection)
        {
            string hashSecret = _config.HashSecret;
            var vnPay = new VnPayLibrary();

            foreach (var (key, value) in collection)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnPay.AddResponseData(key, value);
                }
            }

            var vnpSecureHash = collection.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value; //hash của dữ liệu trả về
            var checkSignature =  vnPay.ValidateSignature(vnpSecureHash, hashSecret); //check Signature
            if (!checkSignature) throw new Exception("Invalid signature");

            return new VnpayCallbackResponse()
            {
                vnp_Amount = vnPay.GetResponseData("vnp_Amount"),
                vnp_BankCode = vnPay.GetResponseData("vnp_BankCode"),
                vnp_BankTranNo = vnPay.GetResponseData("vnp_BankTranNo"),
                vnp_CardType = vnPay.GetResponseData("vnp_CardType"),
                vnp_OrderInfo = vnPay.GetResponseData("vnp_OrderInfo"),
                vnp_PayDate = vnPay.GetResponseData("vnp_PayDate"),
                vnp_ResponseCode = vnPay.GetResponseData("vnp_ResponseCode"),
                vnp_SecureHash = vnPay.GetResponseData("vnp_SecureHash"),
                vnp_TmnCode = vnPay.GetResponseData("vnp_TmnCode"),
                vnp_TransactionNo = vnPay.GetResponseData("vnp_TransactionNo"),
                vnp_TransactionStatus = vnPay.GetResponseData("vnp_TransactionStatus"),
                vnp_TxnRef = vnPay.GetResponseData("vnp_TxnRef")
            };
        }
    }
}
