using Api_Admin.Middlewares;
using Application.Common.Behaviours;
using Application.Common.Dtos;
using Application.Common.Mappers;
using Application.Common.Validations;
using Application.Configuration;
using Application.Services;
using Application.Services.Interfaces;
using Application.SignalR;
using Application.UseCase.AppfeedbackUC.Handlers;
using Application.UseCase.AppfeedbackUC.Queries;
using Application.UseCase.AuthUC.Commands;
using Application.UseCase.AuthUC.Handlers;
using Application.UseCase.DriverUC.Commands;
using Application.UseCase.DriverUC.Handlers;
using Application.UseCase.DriverUC.Queries;
using Application.UseCase.FeeUC.Command;
using Application.UseCase.FeeUC.Handler;
using Application.UseCase.FeeUC.Queries;
using Application.UseCase.ReportUC.Commands;
using Application.UseCase.ReportUC.Handlers;
using Application.UseCase.ReportUC.Queries;
using Application.UseCase.SettingUC.Commands;
using Application.UseCase.SettingUC.Handlers;
using Application.UseCase.SettingUC.Queries;
using Application.UseCase.TripUC.Commands;
using Application.UseCase.TripUC.Handlers;
using Application.UseCase.TripUC.Queries;
using Application.UseCase.UserUC.Commands;
using Application.UseCase.UserUC.Handlers;
using Application.UseCase.UserUC.Queries;
using Application.UseCase.WallettransactionUC.Handlers;
using Application.UseCase.WallettransactionUC.Queries;
using Application.UseCase.WalletUC.Commands;
using Application.UseCase.WalletUC.Handlers;
using AutoMapper;
using Domain.Interfaces;
using FirebaseAdmin;
using FluentValidation;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);


//Add middleware instances
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddTransient<LoggingMiddleware>();
builder.Services.AddTransient<GetUserClaimsMiddleware>();

builder.Services.AddScoped<UserClaims>();
 
// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

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
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = _audience,
        ValidIssuer = _issuer,
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

// Logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
});

// Add dependency injection
builder.Services.AddDbContext<GoShareContext>(options => options.UseNpgsql(GoShareConfiguration.ConnectionString("GoShareAzure")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ISettingService, SettingService>();
builder.Services.AddSingleton<IDriverDocumentService, DriverDocumentService>();

//Add Admin Account
builder.Services.AddSingleton<Admin>(GoShareConfiguration.admin);

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

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//Add handler
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IRequestHandler<VerifyDriverCommand, bool>, VerifyDriverCommandHandler>();
builder.Services.AddScoped<IRequestHandler<AdminAuthCommand, AuthResponse>, AdminAuthCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RefreshTokenCommand, AuthResponse>, RefreshTokenCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GetDriverDocumentQuery, List<DriverDocumentDto>>, GetDriverDocumentQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetAppfeedbacksQuery, PaginatedResult<AppfeedbackDto>>, GetAppfeedbacksHandler>();
builder.Services.AddScoped<IRequestHandler<GetUsersQuery, PaginatedResult<AdminUserResponse>>, GetUsersQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetDriverQuery, PaginatedResult<AdminDriverResponse>>, GetDriverQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetFeedbackQuery, AppfeedbackDto>, GetFeedbackHandler>();
builder.Services.AddScoped<IRequestHandler<CancelTripCommand, TripDto>, CancelTripHandler>();
builder.Services.AddScoped<IRequestHandler<GetUserQuery, UserDto>, GetUserHandler>();
builder.Services.AddScoped<IRequestHandler<GetReportsQuery, PaginatedResult<ReportDto>>, GetReportsHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateReportStatusCommand, ReportDto>, UpdateReportStatusHandler>();
builder.Services.AddScoped<IRequestHandler<GetReportQuery, ReportDto>, GetReportHandler>();
builder.Services.AddScoped<IRequestHandler<GetSystemTransactionQuery, List<WalletTransactionDto>>, GetSystemTransactionHandler>();
builder.Services.AddScoped<IRequestHandler<BanUserCommand, UserDto>, BanUserHandler>();
builder.Services.AddScoped<IRequestHandler<GetTripsQuery, PaginatedResult<TripDto>>, GetTripsHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateWalletBalanceCommand, double>, UpdateWalletBalanceHandler>();
builder.Services.AddScoped<IRequestHandler<UnbanUserCommand, UserDto>, UnbanUserHandler>();
builder.Services.AddScoped<IRequestHandler<GetFeeAndPoliciesQuery, List<FeeDto>>, GetFeeAndPoliciesQueryHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateFeeCommand, bool>, UpdateFeeCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateFeePolicyCommand, bool>, UpdateFeePolicyCommandHandler>();
builder.Services.AddScoped<IRequestHandler<DriverUpdateDocumentCommand, bool>, DriverUpdateDocumentCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GetSettingsQuery, List<SettingDto>>, GetSettingsHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateSettingCommand, SettingDto>, UpdateSettingHandler>();
builder.Services.AddScoped<IRequestHandler<GetUserTransactionQuery, PaginatedResult<WalletTransactionDto>>, GetUserTransactionQueryHandler>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

// Validator
builder.Services.AddScoped<IValidator<BanUserCommand>, BanUserCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateWalletBalanceCommand>, UpdateWalletBalanceCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateSettingCommand>, UpdateSettingCommandValidator>();

var mapperConfig = new MapperConfiguration(cfg =>
{
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
    cfg.AddProfile<FeeProfile>();
    cfg.AddProfile<SettingProfile>();
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

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

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<GetUserClaimsMiddleware>();


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapControllers();

app.UseCors("CorsPolicy");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SignalRHub>("/goshareHub");
});

app.Run();

public partial class Program
{
    private static readonly Mutex _mutex = new Mutex();
}