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
        string choice = "";
        List<string> Menu = new List<string>{
            "Twlio Service - Send otp",
            "Twlio Service - Verify otp",
            "Login Service"
            };
        while (!choice.Equals("-1"))
        {
            try
            {
                Console.WriteLine("TEST SERVICE FROM CONSOLE");
                for (int i = 0; i < Menu.Count; i++)
                {
                    Console.WriteLine(i + "." + Menu[i]);
                };
                choice = Console.ReadLine()!;
                switch (choice)
                {
                    case "clr":
                        {
                            Console.Clear();
                            break;
                        }
                    case "0":
                        {
                            Console.WriteLine("Enter phone number:");
                            string phone = Console.ReadLine()!;
                            Console.WriteLine("Phone number just entered:" + phone);
                            await SendSMSUsingTwilio(phone);
                            break;
                        }
                    case "1":
                        {
                            Console.WriteLine("Enter phone number:");
                            string phone = Console.ReadLine()!;
                            Console.WriteLine("Phone number just entered:" + phone);
                            Console.WriteLine("Enter code:");
                            string code = Console.ReadLine()!;
                            Console.WriteLine("Code just entered:" + code);
                            await VerifyUsingTwilio(phone,code);
                            break;
                        }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("-----------------ERROR---------------");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("--------------------------------------");
            }
        }
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

