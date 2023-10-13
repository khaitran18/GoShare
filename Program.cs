using Api_Mobile.Middlewares;
using Application.Common;
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
using Application.Commands;
using Application.Commands.Handler;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

//Add middlewares
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Initialize Configuration
GoShareConfiguration.Initialize(builder.Configuration);

// Add dependency injection
builder.Services.AddDbContext<postgresContext>(options => options.UseNpgsql(GoShareConfiguration.ConnectionString("GoShareAzure")));
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

// Add Handler
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IRequestHandler<TestQuery, TestDto>, TestQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetAppfeedbacksQuery, PaginatedResult<AppfeedbackDto>>, GetAppfeedbacksHandler>();
builder.Services.AddScoped<IRequestHandler<CreateTripCommand, TripDto>, CreateTripHandler>();
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

builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.Run();
