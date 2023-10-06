using Application.Common;
using Application.Common.Behaviour;
using Application.Common.Dtos;
using Application.Common.Mappers;
using Application.Common.Validations;
using Application.Queries;
using Application.Queries.Handler;
using AutoMapper;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add db context & Unit of work
builder.Services.AddDbContext<postgresContext>(options => options.UseNpgsql(builder.Configuration["ConnectionStrings:GoShareAzure"]));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add Handler
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IRequestHandler<TestQuery,BaseResponse<TestDto>>,TestQueryHandler>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
// Add Validator
builder.Services.AddScoped<IValidator<TestQuery>, TestQueryValidator>();

// Add AutoMapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<TestMapper>();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
