using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Stores;
using ItspServices.pServer.Test.Mock.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ItspServices.pServer.Test
{
    [TestClass]
    public class MvcEndpointTest
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
                    foreach (ServiceDescriptor serviceDescriptor in services.Where(x =>
                    { return x.ServiceType == typeof(IUserRepository) || x.ServiceType == typeof(IRoleRepository); }).ToList())
                        services.Remove(serviceDescriptor);

                    services.AddSingleton<IUserRepository, MockUserRepository>();
                    services.AddSingleton<IRoleRepository, MockRoleRepository>();

                    
                });
            }
        }

        [TestMethod]
        public async Task GetControllerTest()
        {
            HttpClient client = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
            HttpResponseMessage response = await client.GetAsync("/Account/Login");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            response = await client.GetAsync("/Account/Login?returnurl=%2F");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string expectedToken = "action=\"/Account/Login?returnurl=%2F\"";
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains(expectedToken), "Return Url Action not found: " + expectedToken);

            response = await client.PostAsync("/Account/Login?returnurl=%2F", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", "John"),
                new KeyValuePair<string, string>("Password", "Example")
            }));

            content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("<title>Home Page"), "Redirect failed.");
        }
    }
}
