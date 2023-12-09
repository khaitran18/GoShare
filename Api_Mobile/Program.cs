using Api_Mobile.Middlewares;
using Application.Common.Dtos;
using Application.Common.Mappers;
using Application.Common.Validations;
using AutoMapper;
using Domain.Interfaces;
using FirebaseAdmin;
using FluentValidation;
using Google.Apis.Auth.OAuth2;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Application.Configuration;
using Application.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.SignalR;
using Application.Services.Interfaces;
using Google.Cloud.Storage.V1;
using Application.Common.Behaviours;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Application.UseCase.TripUC.Handlers;
using Application.UseCase.UserUC.Commands;
using Application.UseCase.UserUC.Queries;
using Application.UseCase.WalletUC.Queries;
using Application.UseCase.ChatUC.Queries;
using Application.UseCase.WalletUC.Handlers;
using Application.UseCase.UserUC.Handlers;
using Application.UseCase.TripUC.Commands;
using Application.UseCase.PaymentUC.Commands;
using Application.UseCase.WallettransactionUC.Queries;
using Application.UseCase.TripUC.Queries;
using Application.UseCase.PaymentUC.Handler;
using Application.UseCase.DriverUC.Queries;
using Application.UseCase.LocationUC.Queries;
using Application.UseCase.WallettransactionUC.Handlers;
using Application.UseCase.AuthUC.Commands;
using Application.UseCase.AuthUC.Handlers;
using Application.UseCase.AppfeedbackUC.Handlers;
using Application.UseCase.AppfeedbackUC.Commands;
using Application.UseCase.ChatUC.Handlers;
using Application.UseCase.ChatUC.Commands;
using Application.UseCase.DriverUC.Commands;
using Application.UseCase.DriverUC.Handlers;
using Application.UseCase.LocationUC.Commands;
using Application.UseCase.LocationUC.Handlers;


var builder = WebApplication.CreateBuilder(args);

//Add middlewares
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddTransient<LoggingMiddleware>();
builder.Services.AddTransient<GetUserClaimsMiddleware>();
builder.Services.AddTransient<CheckUserVerificationMiddleware>();

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Add class
builder.Services.AddScoped<UserClaims>();

// Logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
});

// Initialize Configuration
GoShareConfiguration.Initialize(builder.Configuration);

//JWT Config

var _key = GoShareConfiguration.jwt.key;
var _expirtyMinutes = GoShareConfiguration.jwt.expiryTime;
var _refreshTokenExpirtyMinutes = GoShareConfiguration.jwt.refreshTokenExpiryTime;
var _issuer = GoShareConfiguration.jwt.issuer;
var _audience = GoShareConfiguration.jwt.audience;

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = _audience,
        ValidIssuer = _issuer,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
        ClockSkew = TimeSpan.FromMinutes(Convert.ToDouble(_expirtyMinutes))
    };
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/goshareHub")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddSingleton<ITokenService>(new TokenService(_key, _expirtyMinutes, _refreshTokenExpirtyMinutes, _issuer, _audience));
builder.Services.AddSingleton<IDriverDocumentService, DriverDocumentService>();
builder.Services.AddSingleton<IDriverService, DriverService>();

//Add VnPayService
builder.Services.AddSingleton<IPaymentService>(new PaymentService(GoShareConfiguration.VnpayConfig));

// Add dependency injection
builder.Services.AddDbContext<GoShareContext>(options => options.UseNpgsql(GoShareConfiguration.ConnectionString("GoShareAzure")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<ISettingService, SettingService>();
builder.Services.AddSingleton<IUserService, UserService>();
// Hangfire
builder.Services.AddHangfire(config => config
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(GoShareConfiguration.ConnectionString("GoShareAzure")))
    .UseFilter(new AutomaticRetryAttribute { Attempts = 0 }));
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 5;
    options.Queues = new[] { "critical", "default" };
});

// Firebase
var credential = GoogleCredential.FromFile(Environment.CurrentDirectory! + "\\" + GoShareConfiguration.FirebaseCredentialFile);

if (FirebaseApp.DefaultInstance == null)
{
    lock (_mutex)
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = credential,
                ProjectId = GoShareConfiguration.FirebaseProjectId
            });
        }
    }
}

StorageClient _storageClient = StorageClient.Create(credential);
builder.Services.AddSingleton<IFirebaseStorage>(new FirebaseStorage(GoShareConfiguration.firebaseBucket, _storageClient));

//SignalR
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials();
    });
});

// Add Handler
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IRequestHandler<CreateTripCommand, TripDto>, CreateTripHandler>();
builder.Services.AddScoped<IRequestHandler<CreateTripForDependentCommand, TripDto>, CreateTripForDependentHandler>();
builder.Services.AddScoped<IRequestHandler<AuthCommand, AuthResponse>, AuthCommandHandler>();
builder.Services.AddScoped<IRequestHandler<AuthDriverCommand, AuthResponse>, AuthDriverCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RegisterCommand, Task>, RegisterCommandHandler>();
builder.Services.AddScoped<IRequestHandler<VerifyCommand, string>, VerifyCommandHandler>();
builder.Services.AddScoped<IRequestHandler<ResendOtpCommand, Task>, ResendOtpCommandHandler>();
builder.Services.AddScoped<IRequestHandler<SetPasscodeCommand, Task>, SetPasscodeCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RefreshTokenCommand, AuthResponse>, RefreshTokenCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RevokeCommand, Task>, RevokeCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateProfilePictureCommand, string>, UpdateProfilePictureHandler>();
builder.Services.AddScoped<IRequestHandler<ConfirmPassengerCommand, TripDto>, ConfirmPassengerHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateFcmTokenCommand, UserDto>, UpdateFcmTokenHandler>();
builder.Services.AddScoped<IRequestHandler<ConfirmPickupPassengerCommand, TripDto>, ConfirmPickupPassengerHandler>();
builder.Services.AddScoped<IRequestHandler<EndTripCommand, TripEndDto>, EndTripHandler>();
builder.Services.AddScoped<IRequestHandler<CalculateFeesForTripCommand, List<CartypeFeeDto>>, CalculateFeesForTripHandler>();
builder.Services.AddScoped<IRequestHandler<GetDependentsQuery, PaginatedResult<UserDto>>, GetDependentsHandler>();
builder.Services.AddScoped<IRequestHandler<DriverRegisterCommand, bool>, DriverRegisterCommandHandler>();
builder.Services.AddScoped<IRequestHandler<AddCarCommand, Guid>, AddCarCommandHandler>();
builder.Services.AddScoped<IRequestHandler<CancelTripCommand, TripDto>, CancelTripHandler>();
builder.Services.AddScoped<IRequestHandler<GetLocationOfDependentCommand, LocationDto>, GetLocationOfDependentHandler>();
builder.Services.AddScoped<IRequestHandler<RateDriverCommand, RatingDto>, RateDriverHandler>();
builder.Services.AddScoped<IRequestHandler<CreateTopUpRequestCommand, string>, CreateTopupRequestCommandHandler>();
builder.Services.AddScoped<IRequestHandler<PaymentCallbackCommand,bool>, PaymentCallbackCommandHandler>();
builder.Services.AddScoped<IRequestHandler<SendMessageCommand,Task>, SendMessageHandler>();
builder.Services.AddScoped<IRequestHandler<GetMessagesQuery,List<ChatDto>>, GetMessageQueryHandler>();
builder.Services.AddScoped<IRequestHandler<DriverActivateCommand, bool>, DriverActivateHandler>();
builder.Services.AddScoped<IRequestHandler<DriverDeactivateCommand, bool>, DriverDeactivateHandler>();
builder.Services.AddScoped<IRequestHandler<GetLocationQuery, List<LocationDto>>, GetLocationQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetCurrentTripQuery, TripDto>, GetCurrentTripQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetTripQuery, TripDto?>, GetTripQueryHandler>();
builder.Services.AddScoped<IRequestHandler<CreatePlannedDestinationCommand, LocationDto>, CreatePlannedDestinationHandler>();
builder.Services.AddScoped<IRequestHandler<DriverUpdateLocationCommand, LocationDto>, DriverUpdateLocationHandler>();
builder.Services.AddScoped<IRequestHandler<CreateDependentCommand, UserDto>, CreateDependentCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GetWalletBalanceQuery, double>, GetWalletBalanceQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetUserTransactionQuery, List<WalletTransactionDto>>, GetUserTransactionQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetTripHistoryQuery, List<TripDto>>, GetTripHistoryHandler>();
builder.Services.AddScoped<IRequestHandler<CreateFeedbackCommand, AppfeedbackDto>, CreateFeedbackHandler>();
builder.Services.AddScoped<IRequestHandler<DriverUpdateDocumentCommand, bool>, DriverUpdateDocumentCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GetDriverInformationQuery, DriverInformationResponse>, GetDriverInformationHandler>();
builder.Services.AddScoped<IRequestHandler<DeletePlannedDestinationCommand, bool>, DeletePlannedDestinationHandler>();
builder.Services.AddScoped<IRequestHandler<GetUserQuery, UserDto>, GetUserHandler>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>))
    .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));



// Fluent Validation

builder.Services.AddScoped<IValidator<CreateTripCommand>, CreateTripCommandValidator>();
builder.Services.AddScoped<IValidator<AddCarCommand>, AddCarCommandValidator>();
builder.Services.AddScoped<IValidator<AuthCommand>, AuthCommandValidator>();
builder.Services.AddScoped<IValidator<CalculateFeesForTripCommand>, CalculateFeesForTripValidator>();
builder.Services.AddScoped<IValidator<ConfirmPickupPassengerCommand>, ConfirmPickupPassengerValidator>();
builder.Services.AddScoped<IValidator<CreatePlannedDestinationCommand>, CreatePlannedDestinationValidator>();
builder.Services.AddScoped<IValidator<CreateTopUpRequestCommand>, CreateTopUpRequestValidator>();
builder.Services.AddScoped<IValidator<CreateTripCommand>, CreateTripCommandValidator>();
builder.Services.AddScoped<IValidator<CreateTripForDependentCommand>, CreateTripForDependentCommandValidator>();
builder.Services.AddScoped<IValidator<DriverRegisterCommand>, DriverRegisterCommandValidator>();
builder.Services.AddScoped<IValidator<DriverUpdateLocationCommand>, DriverUpdateLocationCommandValidator>();
builder.Services.AddScoped<IValidator<EndTripCommand>, EndtripCommandValidator>();
builder.Services.AddScoped<IValidator<RegisterCommand>, RegisterCommandValidator>();
builder.Services.AddScoped<IValidator<CreateDependentCommand>, CreateDependentCommandValidator>();
builder.Services.AddScoped<IValidator<CreateFeedbackCommand>, CreateFeedbackCommandValidator>();

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
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


//Add twilio
builder.Services.AddSingleton<ITwilioVerification>(new TwilioVerification(GoShareConfiguration.TwilioAccount));

//Add SpeedSMSAPI
builder.Services.AddSingleton<SpeedSMS>();
builder.Services.AddScoped<ISpeedSMSAPI, SpeedSMSAPI>();
builder.Services.AddSingleton<ISpeedSMSAPI>(new SpeedSMSAPI(GoShareConfiguration.SpeedSMSAccount));

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
var app = builder.Build();

// Load settings
var settingService = app.Services.GetRequiredService<ISettingService>();
settingService.LoadSettings().Wait();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<GetUserClaimsMiddleware>();
app.UseMiddleware<CheckUserVerificationMiddleware>();

app.UseCors("CorsPolicy");

app.UseHangfireDashboard("/hangfire");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SignalRHub>("/goshareHub");
});
 
app.Run();

public partial class Program
{
    private static readonly Mutex _mutex = new Mutex();
}