using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
                    //foreach (ServiceDescriptor serviceDescriptor in services.Where(x => x.ServiceType == typeof(ILoggerProvider)).ToList())
                    //  services.Remove(serviceDescriptor);

                });
            }
        }

        [TestMethod]
        public async Task GetControllerTest()
        {
            HttpResponseMessage response = await new WebApplicationFactory().CreateClient().GetAsync("/Account/Login");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
