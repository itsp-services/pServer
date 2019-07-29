using ItspServices.pServer.Abstraction.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ItspServices.pServer.Test
{

    [TestClass]
    public class AuthorizedEnpointTest
    {

        static HttpClient Client { get; set; }

        [ClassInitialize]
        public static async Task ClassInit(TestContext context)
        {
            Client = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
            await Client.PostAsync("/Account/Login", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", "Foo"),
                new KeyValuePair<string, string>("Password", "Bar123456789")
            }));
        }

        [TestMethod]
        public async Task ProtectedDataGetRootFolderEndpoint()
        {
            var response = await Client.GetAsync("/api/protecteddata/folder");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task ProtectedDataGetRootFolder()
        {
            var response = await Client.GetAsync("/api/protecteddata/folder");
            string content = await response.Content.ReadAsStringAsync();
            JToken token = JToken.Parse(content);
            Assert.AreEqual(token["id"].Value<int>(), 0);
            Assert.AreEqual(token["name"].Value<string>(), "root");
        }

        [TestMethod]
        public async Task ProtectedDataGetFolderById()
        {
            var response = await Client.GetAsync("/api/protecteddata/folder/1");
            string content = await response.Content.ReadAsStringAsync();
            JToken token = JToken.Parse(content);
            Assert.AreEqual(token["id"].Value<int>(), 1);
            Assert.AreEqual(token["name"].Value<string>(), "foo");
        }

        [TestMethod]
        public async Task ProtectedDataGetById()
        {
            ProtectedData sampleData = new ProtectedData()
            {
                Id = 0,
                OwnerId = 0,
                Name = "FooData"
            };
            sampleData.Data = Encoding.UTF8.GetBytes("BarData");

            var response = await Client.GetAsync("/api/protecteddata/data/0");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            ProtectedData data = JsonConvert.DeserializeObject<ProtectedData>(content);

            AssertDataEqual(sampleData, data);
        }

        [TestMethod]
        public async Task NotFoundGetDataById()
        {
            var response = await Client.GetAsync("/api/protecteddata/data/99");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        private void AssertDataEqual(ProtectedData expected, ProtectedData actual)
        {
            Assert.AreEqual(expected.Id, actual.Id, "Id did not match.");
            Assert.AreEqual(expected.OwnerId, actual.OwnerId, "Owner did not match.");
            Assert.AreEqual(expected.Name, actual.Name, "Name did not match.");
            CollectionAssert.AreEquivalent(expected.Data, actual.Data, "Data did not match.");
        }

    }
}
