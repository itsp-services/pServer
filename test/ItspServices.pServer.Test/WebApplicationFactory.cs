using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Test.Mock.Repository;

namespace ItspServices.pServer.Test
{

    class WebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder() =>
            Program.CreateWebHostBuilder(new string[0]);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureTestServices(services =>
            {
                foreach (ServiceDescriptor serviceDescriptor in services.Where(x => x.ServiceType == typeof(IRepositoryManager)).ToList())
                    services.Remove(serviceDescriptor);

                services.AddSingleton(typeof(IRepositoryManager), typeof(MockRepositoryManager));
            });
        }
    }
}
