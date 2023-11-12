using Api_Mobile.Middlewares;
using Application.Common.Dtos;
using Application.Common.Mappers;
using Application.Common.Validations;
using Application.Queries;
using Application.Queries.Handler;
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
using Application.Commands;
using Application.Commands.Handlers;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.SignalR;
using Application.Services.Interfaces;
using Google.Cloud.Storage.V1;
using Application.Common.Behaviours;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Add middlewares
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddTransient<LoggingMiddleware>();
builder.Services.AddTransient<GetUserClaimsMiddleware>();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Add class
builder.Services.AddScoped<UserClaims>();

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
});
builder.Services.AddSingleton<ITokenService>(new TokenService(_key,_expirtyMinutes,_refreshTokenExpirtyMinutes,_issuer,_audience));
builder.Services.AddSingleton<IDriverDocumentService, DriverDocumentService>();

// Add dependency injection
builder.Services.AddDbContext<GoShareContext>(options => options.UseNpgsql(GoShareConfiguration.ConnectionString("GoShareAzure")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<ISettingService, SettingService>();

// Hangfire
builder.Services.AddHangfire(config => config
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(GoShareConfiguration.ConnectionString("GoShareAzure")))
    .UseFilter(new AutomaticRetryAttribute { Attempts = 5 }));
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 5;
    options.Queues = new[] { "critical", "default" };
});

var cts = new CancellationTokenSource();
builder.Services.AddSingleton(cts);

// Firebase
var credential = GoogleCredential.FromFile(Environment.CurrentDirectory! +  "\\" +GoShareConfiguration.FirebaseCredentialFile);
FirebaseApp.Create(new AppOptions
{
    Credential = credential,
    ProjectId = GoShareConfiguration.FirebaseProjectId
});
StorageClient _storageClient = StorageClient.Create(credential);
builder.Services.AddSingleton<IFirebaseStorage>(new FirebaseStorage(GoShareConfiguration.firebaseBucket, _storageClient));

//SignalR
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
});

// Add Handler
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IRequestHandler<TestQuery, TestDto>, TestQueryHandler>();
builder.Services.AddScoped<IRequestHandler<CreateTripCommand, TripDto>, CreateTripHandler>();
builder.Services.AddScoped<IRequestHandler<CreateTripForDependentCommand, TripDto>, CreateTripForDependentHandler>();
builder.Services.AddScoped<IRequestHandler<AuthCommand,TokenResponse>, AuthCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RegisterCommand,Task>, RegisterCommandHandler>();
builder.Services.AddScoped<IRequestHandler<VerifyCommand, string>, VerifyCommandHandler>();
builder.Services.AddScoped<IRequestHandler<ResendOtpCommand, Task>, ResendOtpCommandHandler>();
builder.Services.AddScoped<IRequestHandler<SetPasscodeCommand, Task>, SetPasscodeCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RefreshTokenCommand, string>, RefreshTokenCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RevokeCommand, Task>, RevokeCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateProfilePictureCommand, string>, UpdateProfilePictureHandler>();
builder.Services.AddScoped<IRequestHandler<ConfirmPassengerCommand, TripDto>, ConfirmPassengerHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateFcmTokenCommand, UserDto>, UpdateFcmTokenHandler>();
builder.Services.AddScoped<IRequestHandler<ConfirmPickupPassengerCommand, TripDto>, ConfirmPickupPassengerHandler>();
builder.Services.AddScoped<IRequestHandler<EndTripCommand, TripDto>, EndTripHandler>();
builder.Services.AddScoped<IRequestHandler<CalculateFeesForTripCommand, List<CartypeFeeDto>>, CalculateFeesForTripHandler>();
builder.Services.AddScoped<IRequestHandler<GetDependentsQuery, PaginatedResult<UserDto>>, GetDependentsHandler>();
builder.Services.AddScoped<IRequestHandler<DriverRegisterCommand, bool>, DriverRegisterCommandHandler>();
builder.Services.AddScoped<IRequestHandler<AddCarCommand, Guid>, AddCarCommandHandler>();
builder.Services.AddScoped<IRequestHandler<CancelTripCommand, TripDto>, CancelTripHandler>();
builder.Services.AddScoped<IRequestHandler<GetLocationOfDependentCommand, LocationDto>, GetLocationOfDependentHandler>();
builder.Services.AddScoped<IRequestHandler<RateDriverCommand, RatingDto>, RateDriverHandler>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>))
    .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));



// Fluent Validation
builder.Services.AddScoped<IValidator<TestQuery>, TestQueryValidator>();
builder.Services.AddScoped<IValidator<CreateTripCommand>, CreateTripCommandValidator>();

// Add AutoMapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<TestMapper>();
    cfg.AddProfile<AppfeedbackProfile>();
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<TripProfile>();
    cfg.AddProfile<CarProfile>();
    cfg.AddProfile<LocationProfile>();
    cfg.AddProfile<CartypeProfile>();
    cfg.AddProfile<DriverdocumentProfile>();
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

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
        {
            new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
            {
                SslRedirect = false,
                RequireSsl = false,
                LoginCaseSensitive = true,
                Users = new []
                {
                    new BasicAuthAuthorizationUser
                    {
                        Login = "admin",
                        PasswordClear = "1"
                    }
                }
            })
        }
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SignalRHub>("/goshareHub");
});

app.Services.GetService<IHostApplicationLifetime>()?.ApplicationStopping.Register(cts.Cancel);

app.Run();
