using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_Mobile.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IMediator> MediatorMock;

        public CustomWebApplicationFactory()
        {
            MediatorMock = new Mock<IMediator>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(MediatorMock.Object);
            });
        }
    }
}
