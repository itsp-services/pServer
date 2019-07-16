using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Test.Mock.Repository;

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
                    foreach (ServiceDescriptor serviceDescriptor in services.Where(x => x.ServiceType == typeof(IRepository)).ToList())
                        services.Remove(serviceDescriptor);

                    services.AddSingleton(typeof(IRepository), typeof(MockRepository));
                });
            }
        }

        [TestMethod]
        public async Task GetAccountControllerLoginTest()
        {
            HttpClient client = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
            Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/Account/Login")).StatusCode);
        }

        [TestMethod]
        public async Task ReturnUrlFormTest()
        {
            HttpClient client = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
            HttpResponseMessage response = await client.GetAsync("/Account/Login?returnurl=%2F");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string expectedToken = "action=\"/Account/Login?returnurl=%2F\"";
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains(expectedToken), "Return Url Action not found: " + expectedToken);
        }

        [TestMethod]
        public async Task PostCredentialsLoginTest()
        {
            HttpClient client = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });

            HttpResponseMessage response = await client.PostAsync("/Account/Login?returnurl=%2F", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", "Foo"),
                new KeyValuePair<string, string>("Password", "Bar")
            }));

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("<title>Home Page"), "Redirect failed.");
        }

        [TestMethod]
        public async Task GetAccountControllerRegisterTest()
        {
            HttpClient client = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
            Assert.AreEqual(HttpStatusCode.OK, (await client.GetAsync("/Account/Register")).StatusCode);
        }

        [TestMethod]
        public async Task PostCredentialsRegisterTest()
        {
            HttpClient client = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });

            HttpResponseMessage response = await client.PostAsync("/Account/Login?returnurl=%2F", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", "John"),
                new KeyValuePair<string, string>("Password", "Doe")
            }));
        }
    }
}
