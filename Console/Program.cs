using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.Configuration;
using Application.Services;
using Application.Services.Interfaces;
using Domain.DataModels;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Reflection;
using Twilio.Exceptions;

partial class Program
{
    static async Task Main(string[] args)
    {
        IUnitOfWork _unitOfWork;
        GoShareContext _context;
        (_unitOfWork,_context) = Configure();
        string phone = "+919191919191";
        User u = _context.Users.FirstOrDefault(u=>u.Phone.Equals(phone))!;
        string PasscodeResetToken = OtpUtils.Generate();
        Console.WriteLine(PasscodeResetToken);
        u!.Isverify = true;
        u!.PasscodeResetToken = PasswordHasher.Hash(PasscodeResetToken);
        u!.PasscodeResetTokenExpiryTime = DateTimeUtilities.GetDateTimeVnNow().AddMinutes(60);
        await _unitOfWork.Save();
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

    private static (IUnitOfWork, GoShareContext) Configure()
    {
        var builder = new ConfigurationBuilder()
        .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true);
        var configuration = builder.Build();
        GoShareConfiguration.Initialize(configuration);
        var services = new ServiceCollection();
        services.AddDbContext<GoShareContext>(options => options.UseNpgsql(GoShareConfiguration.ConnectionString("GoShareAzure")));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        var serviceProvider = services.BuildServiceProvider();
        var _unitOfWork = serviceProvider.GetService<IUnitOfWork>();
        var _context = serviceProvider.GetService<GoShareContext>();
        return (_unitOfWork!, _context!);
    }

}

