using Api_Mobile.Middlewares;
using Application.Common.Behaviour;
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
using Application.Service;
using Application.SignalR;

var builder = WebApplication.CreateBuilder(args);

//Add middlewares
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
// Add services to the container.
builder.Services.AddControllers();

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
        ClockSkew = TimeSpan.FromMinutes(Convert.ToDouble(_expirtyMinutes))
    };
});
builder.Services.AddSingleton<ITokenService>(new TokenService(_key,_expirtyMinutes,_refreshTokenExpirtyMinutes,_issuer,_audience));

// Add dependency injection
builder.Services.AddDbContext<GoShareContext>(options => options.UseNpgsql(GoShareConfiguration.ConnectionString("GoShareAzure")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Hangfire
builder.Services.AddHangfire(config => config
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(GoShareConfiguration.ConnectionString("GoShareAzure")))
    .UseFilter(new AutomaticRetryAttribute { Attempts = 5 }));
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 5;
    options.Queues = new[] { "critical", "default" };
});

// Firebase
var credential = GoogleCredential.FromFile(GoShareConfiguration.FirebaseCredentialFile);
FirebaseApp.Create(new AppOptions
{
    Credential = credential,
    ProjectId = GoShareConfiguration.FirebaseProjectId
});

// SignalR
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
});

// Add Handler
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IRequestHandler<TestQuery, TestDto>, TestQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetAppfeedbacksQuery, PaginatedResult<AppfeedbackDto>>, GetAppfeedbacksHandler>();
builder.Services.AddScoped<IRequestHandler<CreateTripCommand, TripDto>, CreateTripHandler>();
builder.Services.AddScoped<IRequestHandler<AuthCommand,TokenResponse>, AuthCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RegisterCommand,Task>, RegisterCommandHandler>();
builder.Services.AddScoped<IRequestHandler<VerifyCommand, string>, VerifyCommandHandler>();
builder.Services.AddScoped<IRequestHandler<ResendOtpCommand, Task>, ResendOtpCommandHandler>();
builder.Services.AddScoped<IRequestHandler<SetPasscodeCommand, Task>, SetPasscodeCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RefreshTokenCommand, string>, RefreshTokenCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RevokeCommand, Task>, RevokeCommandHandler>();
builder.Services.AddScoped<IRequestHandler<ConfirmPassengerCommand, bool>, ConfirmPassengerHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateFcmTokenCommand, UserDto>, UpdateFcmTokenHandler>();
builder.Services.AddScoped<IRequestHandler<ConfirmPickupPassengerCommand, TripDto>, ConfirmPickupPassengerHandler>();
builder.Services.AddScoped<IRequestHandler<EndTripCommand, TripDto>, EndTripHandler>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Add Validator
builder.Services.AddScoped<IValidator<TestQuery>, TestQueryValidator>();

// Add AutoMapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<TestMapper>();
    cfg.AddProfile<AppfeedbackProfile>();
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<TripProfile>();
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
// Add Behaviour
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

//Add twilio
builder.Services.AddSingleton<Application.Configuration.Twilio>();
builder.Services.AddScoped<ITwilioVerification, TwilioVerification>();
//builder.Services.AddSingleton<IVerification>(new Verification(builder.Configuration.GetSection("Twilio").Get<Application.Common.Dtos.Twilio>()));
builder.Services.AddSingleton<ITwilioVerification>(new TwilioVerification(GoShareConfiguration.TwilioAccount));

//Add SpeedSMSAPI
builder.Services.AddSingleton<SpeedSMS>();
builder.Services.AddScoped<ISpeedSMSAPI, SpeedSMSAPI>();
builder.Services.AddSingleton<ISpeedSMSAPI>(new SpeedSMSAPI(GoShareConfiguration.SpeedSMSAccount));

builder.Services.AddSwaggerGen();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseHangfireDashboard();

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SignalRHub>("/goshareHub");
});

app.Run();
