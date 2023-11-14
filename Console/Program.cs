using Application.Commands;
using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.Configuration;
using Application.Services;
using Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Reflection;
using Twilio.Exceptions;

partial class Program
{
    static async Task Main(string[] args)
    {
        Configure();
    }
    private static async Task SendSMSUsingTwilio(string phone)
    {
        ITwilioVerification _service = new TwilioVerification(GoShareConfiguration.TwilioAccount);
        var response = await _service.StartVerificationAsync(phone, "sms");
        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(response))
        {
            string name = descriptor.Name;
            object? value = descriptor.GetValue(response);
            Console.WriteLine("{0}={1}", name, value);
        }
    }
    private static async Task VerifyUsingTwilio(string phone,string code)
    {
        try
        {
            ITwilioVerification _service = new TwilioVerification(GoShareConfiguration.TwilioAccount);
            var response = await _service.CheckVerificationAsync(phone, code);
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(response))
            {
                string name = descriptor.Name;
                object? value = descriptor.GetValue(response);
                Console.WriteLine("{0}={1}", name, value);
            }
        }
        catch (TwilioException e)
        {
            throw e;
        }

    }

    //private static async Task CreatePaymentUrl(OrderInfo order)
    //{
    //    //Get Config Info
    //    string vnp_Returnurl = "facebook.com"; //URL nhan ket qua tra ve 
    //    string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; //URL thanh toan cua VNPAY 
    //    string vnp_TmnCode = "HVXA1C4M"; //Ma định danh merchant kết nối (Terminal Id)
    //    string vnp_HashSecret = "QVWQGRJGRLSZCZOLGXIVWIIMCYLSUAVT"; //Secret Key

    //    //Get payment input
        

    //    //Save order to db

    //    //Build URL for VNPAY
    //    VnPayLibrary vnpay = new VnPayLibrary();
    //    vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
    //    vnpay.AddRequestData("vnp_Command", "pay");
    //    vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
    //    vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
    //    vnpay.AddRequestData("vnp_BankCode", "");
    //    vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
    //    vnpay.AddRequestData("vnp_CurrCode", "VND");
    //    vnpay.AddRequestData("vnp_IpAddr", "115.79.219.34");
    //    vnpay.AddRequestData("vnp_Locale", "vn");
    //    vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
    //    vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
    //    vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
    //    vnpay.AddRequestData("vnp_TxnRef", Guid.NewGuid().ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

    //    //Add Params of 2.1.0 Version
    //    //Billing

    //    string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
    //    Console.WriteLine("Payment Url: " + paymentUrl);
    //}

    //private static async Task CreateIpnUrl(OrderInfo order)
    //{
    //    //Get Config Info
    //    string vnp_Returnurl = "facebook.com"; //URL nhan ket qua tra ve 
    //    string vnp_Url = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction"; //URL thanh toan cua VNPAY 
    //    string vnp_TmnCode = "HVXA1C4M"; //Ma định danh merchant kết nối (Terminal Id)
    //    string vnp_HashSecret = "QVWQGRJGRLSZCZOLGXIVWIIMCYLSUAVT"; //Secret Key

    //    //Save order to db

    //    //Build URL for VNPAY
    //    VnPayLibrary vnpay = new VnPayLibrary();
    //    vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
    //    vnpay.AddRequestData("vnp_Command", "querydr");
    //    vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
    //    vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
    //    vnpay.AddRequestData("vnp_BankCode", "");
    //    vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
    //    vnpay.AddRequestData("vnp_CurrCode", "VND");
    //    vnpay.AddRequestData("vnp_IpAddr", "115.79.219.34");
    //    vnpay.AddRequestData("vnp_Locale", "vn");
    //    vnpay.AddRequestData("vnp_OrderInfo", "Tra cuu ket qua giao dich:" + order.OrderId);
    //    vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
    //    vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
    //    vnpay.AddRequestData("vnp_TxnRef", Guid.NewGuid().ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

    //    //Add Params of 2.1.0 Version
    //    //Billing

    //    string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
    //    Console.WriteLine("Payment Url: " + paymentUrl);
    //}

    private static void Configure()
    {
        var builder = new ConfigurationBuilder()
        .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true);
        var configuration = builder.Build();
        GoShareConfiguration.Initialize(configuration);
    }

}

