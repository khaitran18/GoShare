using Application.Commands;
using Application.Configuration;
using Application.Services;
using Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Reflection;
partial class Program
{
    static async Task Main(string[] args)
    {
        Configure();
        string choice = "";
        List<string> Menu = new List<string>{
            "Twlio Service",
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
        Console.WriteLine(await _service.StartVerificationAsync(phone, "sms"));
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

