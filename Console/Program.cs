using Application.Common.Dtos;
using Application.Common.Mappers;
using Application.Common.Utilities;
using Application.Configuration;
using Application.Services;
using Application.Services.Interfaces;
using Application.UseCase.DriverUC.Commands;
using Application.UseCase.DriverUC.Handlers;
using AutoMapper;
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
        IMediator _mediator;
        (_unitOfWork,_context,_mediator) = Configure();
        ConfirmPickupPassengerCommand command = new ConfirmPickupPassengerCommand();
        command.TripId = new Guid("342e3435-66e5-4d65-a6a1-bb8381201362");
        command.DriverLongitude = (decimal)10.8441264;
        command.DriverLatitude = (decimal)106.7988646;
        command.Image = null;
        await _mediator.Send(command);
        //string phone = "+919191919191";
        //User u = _context.Users.FirstOrDefault(u=>u.Phone.Equals(phone))!;
        //string PasscodeResetToken = OtpUtils.Generate();
        //Console.WriteLine(PasscodeResetToken);
        //u!.Isverify = true;
        //u!.PasscodeResetToken = PasswordHasher.Hash(PasscodeResetToken);
        //u!.PasscodeResetTokenExpiryTime = DateTimeUtilities.GetDateTimeVnNow().AddMinutes(60);


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

    private static async Task CheckTransaction()
    {
        var response = new List<WalletMonthStatistic>();
        var transactions = new List<Wallettransaction>();
        transactions.Add(new Wallettransaction
        {
            Id = Guid.NewGuid(),
            WalletId = Guid.NewGuid(),
            Amount = -50.0,
            PaymentMethod = Domain.Enumerations.PaymentMethod.WALLET,
            Status = Domain.Enumerations.WalletTransactionStatus.SUCCESSFULL,
            Type = Domain.Enumerations.WalletTransactionType.TOPUP,
            CreateTime = DateTime.Now.AddMonths(-1),
            UpdatedTime = DateTime.Now.AddHours(-2)
        });
        transactions.Add(new Wallettransaction
        {
            Id = Guid.NewGuid(),
            WalletId = Guid.NewGuid(),
            Amount = 50.0,
            PaymentMethod = Domain.Enumerations.PaymentMethod.WALLET,
            Status = Domain.Enumerations.WalletTransactionStatus.SUCCESSFULL,
            Type = Domain.Enumerations.WalletTransactionType.TOPUP,
            CreateTime = DateTime.Now.AddMonths(-1),
            UpdatedTime = DateTime.Now.AddHours(-2)
        });
        transactions.Add(new Wallettransaction
        {
            Id = Guid.NewGuid(),
            WalletId = Guid.NewGuid(),
            Amount = 50.0,
            PaymentMethod = Domain.Enumerations.PaymentMethod.WALLET,
            Status = Domain.Enumerations.WalletTransactionStatus.SUCCESSFULL,
            Type = Domain.Enumerations.WalletTransactionType.TOPUP,
            CreateTime = DateTime.Now.AddMonths(-1),
            UpdatedTime = DateTime.Now.AddHours(-2)
        });
        transactions.Add(new Wallettransaction
        {
            Id = Guid.NewGuid(),
            WalletId = Guid.NewGuid(),
            Amount = 50.0,
            PaymentMethod = Domain.Enumerations.PaymentMethod.WALLET,
            Status = Domain.Enumerations.WalletTransactionStatus.SUCCESSFULL,
            Type = Domain.Enumerations.WalletTransactionType.TOPUP,
            CreateTime = DateTime.Now.AddMonths(-1),
            UpdatedTime = DateTime.Now.AddHours(-2)
        });
        transactions.Add(new Wallettransaction
        {
            Id = Guid.NewGuid(),
            WalletId = Guid.NewGuid(),
            Amount = 50.0,
            PaymentMethod = Domain.Enumerations.PaymentMethod.WALLET,
            Status = Domain.Enumerations.WalletTransactionStatus.SUCCESSFULL,
            Type = Domain.Enumerations.WalletTransactionType.TOPUP,
            CreateTime = DateTime.Now.AddMonths(0),
            UpdatedTime = DateTime.Now.AddHours(-2)
        });
        int numberOfMonth = 6;
        int numOfWeekInMonth = 4;
        var time = DateTimeUtilities.GetDateTimeVnNow().AddMonths(1);
        for (int i = numberOfMonth; i >= 0; i--)
        {
            var t = time.AddMonths((-1) * i);
            var monthTransactions = transactions.Where(u => u.CreateTime.Month == t.Month);
            WalletMonthStatistic stat = new WalletMonthStatistic();
            stat.Month = t.Month;
            stat.MonthTotal = monthTransactions.Sum(u => u.Amount);
            stat.WeekAverage = stat.MonthTotal / numOfWeekInMonth;
            if (response.Count != 0 && response.Last().MonthTotal != 0) stat.CompareToLastMonth = monthTransactions.Sum(u => u.Amount) / response.Last().MonthTotal * 100;
            if (i != numberOfMonth) response.Add(stat);
        }
        foreach (var item in response)
        {
            Console.WriteLine("----------------------");
            Console.WriteLine(item.Month);
            Console.WriteLine(item.MonthTotal);
            Console.WriteLine(item.WeekAverage);
            Console.WriteLine(item.CompareToLastMonth);
        }
        Console.ReadLine();
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

    private static (IUnitOfWork, GoShareContext,IMediator) Configure()
    {
        var builder = new ConfigurationBuilder()
        .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true);
        var configuration = builder.Build();
        GoShareConfiguration.Initialize(configuration);
        var services = new ServiceCollection();
        services.AddDbContext<GoShareContext>(options => options.UseNpgsql(GoShareConfiguration.ConnectionString("GoShareAzure")));
        services.AddScoped<IRequestHandler<ConfirmPickupPassengerCommand, TripDto>, ConfirmPickupPassengerHandler>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        // Add AutoMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TestMapper>();
            cfg.AddProfile<AppfeedbackProfile>();
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<TripProfile>();
            cfg.AddProfile<CarProfile>();
            cfg.AddProfile<ChatProfile>();
            cfg.AddProfile<LocationProfile>();
            cfg.AddProfile<CartypeProfile>();
            cfg.AddProfile<DriverdocumentProfile>();
            cfg.AddProfile<WallettransactionProfile>();
            cfg.AddProfile<RatingProfile>();
            cfg.AddProfile<ReportProfile>();
            cfg.AddProfile<TripImageProfile>();
        });
        var mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);
        services.AddSingleton<ISettingService, SettingService>();
        var serviceProvider = services.BuildServiceProvider();
        var _unitOfWork = serviceProvider.GetService<IUnitOfWork>();
        var _context = serviceProvider.GetService<GoShareContext>();
        var _mediator = serviceProvider.GetService<IMediator>();
        // Load settings
        var settingService = serviceProvider.GetRequiredService<ISettingService>();
        settingService.LoadSettings().Wait();
        return (_unitOfWork!, _context!,_mediator!);
    }

}

