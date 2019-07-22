using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ItspServices.pServer.Test
{
    [TestClass]
    public class MvcEndpointTest
    {
        public HttpClient Client { get; set; }

        [TestInitialize]
        public void Init()
        {
            Client = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
        }

        [TestMethod]
        public async Task GetAccountControllerLoginTest()
        {
            Assert.AreEqual(HttpStatusCode.OK, (await Client.GetAsync("/Account/Login")).StatusCode);
        }

        [TestMethod]
        public async Task ReturnUrlFormTest()
        {
            HttpResponseMessage response = await Client.GetAsync("/Account/Login?returnurl=%2F");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string expectedToken = "action=\"/Account/Login?returnurl=%2F\"";
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains(expectedToken), "Return Url Action not found: " + expectedToken);
        }

        [TestMethod]
        public async Task PostCredentialsLoginTest()
        {
            HttpResponseMessage response = await Client.PostAsync("/Account/Login?returnurl=%2F", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", "Foo"),
                new KeyValuePair<string, string>("Password", "Bar123456789")
            }));

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("<title>Home Page"), "Redirect failed.");
        }

        [TestMethod]
        public async Task GetAccountControllerRegisterTest()
        {
            Assert.AreEqual(HttpStatusCode.OK, (await Client.GetAsync("/Account/Register")).StatusCode);
        }

        [TestMethod]
        public async Task PostCredentialsRegisterTest()
        {
            HttpResponseMessage response = await Client.PostAsync("/Account/Register?returnurl=%2F", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", "John"),
                new KeyValuePair<string, string>("Password", "Doe12345!"),
                new KeyValuePair<string, string>("ConfirmPassword", "Doe12345!")
            }));

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("<title>Home Page"), "Redirect failed.");
        }
    }
}
