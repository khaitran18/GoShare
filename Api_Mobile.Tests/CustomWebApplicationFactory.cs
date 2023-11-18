using Application.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Api_Mobile.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IMediator> MediatorMock;
        public Mock<ITokenService> TokenServiceMock;

        public CustomWebApplicationFactory()
        {
            MediatorMock = new Mock<IMediator>();
            TokenServiceMock = new Mock<ITokenService>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(MediatorMock.Object);
                services.AddSingleton(TokenServiceMock.Object);
            });
        }

        public override async ValueTask DisposeAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(10)); // Increase timeout for stopping hosted services
            await base.DisposeAsync();
        }
    }
}
