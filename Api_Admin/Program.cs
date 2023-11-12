using Api_Admin.Middlewares;
using Application.Commands;
using Application.Commands.Handlers;
using Application.Common.Dtos;
using Application.Common.Mappers;
using Application.Configuration;
using Application.Queries;
using Application.Queries.Handler;
using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces;
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

var builder = WebApplication.CreateBuilder(args);


//Add middleware instances
builder.Services.AddTransient<LoggingMiddleware>();
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
        ValidAudience = _audience,
        ValidIssuer = _issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
        ClockSkew = TimeSpan.FromMinutes(Convert.ToDouble(_expirtyMinutes))
    };
});
builder.Services.AddSingleton<ITokenService>(new TokenService(_key, _expirtyMinutes, _refreshTokenExpirtyMinutes, _issuer, _audience));

// Add dependency injection
builder.Services.AddDbContext<GoShareContext>(options => options.UseNpgsql(GoShareConfiguration.ConnectionString("GoShareAzure")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


//Add Admin Account
builder.Services.AddSingleton<Admin>(GoShareConfiguration.admin);


//Add handler
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IRequestHandler<VerifyDriverCommand, bool>, VerifyDriverCommandHandler>();
builder.Services.AddScoped<IRequestHandler<AdminAuthCommand, TokenResponse>, AdminAuthCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RefreshTokenCommand, string>, RefreshTokenCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GetDriverDocumentQuery, List<DriverDocumentDto>>, GetDriverDocumentQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetAppfeedbacksQuery, PaginatedResult<AppfeedbackDto>>, GetAppfeedbacksHandler>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));



var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<DriverdocumentProfile>();
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


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
