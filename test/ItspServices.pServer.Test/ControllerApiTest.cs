using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace ItspServices.pServer.Test
{
    [TestClass]
    public class ControllerApiTest
    {
        public HttpClient Client { get; set; }

        [TestInitialize]
        public void Init()
        {
            Client = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
        }

        [TestMethod]
        public async Task GetProtectedDataControllerTest()
        {
            Assert.AreEqual(HttpStatusCode.OK, (await Client.GetAsync("/api/protecteddata")).StatusCode);
        }

        [TestMethod]
        public async Task GetRootFolderTest()
        {
            HttpResponseMessage response = await Client.GetAsync("/ProtectedData/GetFolder");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            JToken token = JToken.Parse(content);
            Assert.AreEqual(token["name"].Value<string>(), "root");
        }
    }
}
