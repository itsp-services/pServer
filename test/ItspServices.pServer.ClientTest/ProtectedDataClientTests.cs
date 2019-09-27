using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ItspServices.pServer.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ItspServices.pServer.ClientTest
{
    [TestClass]
    public class ProtectedDataClientTests
    {
        #region nested classes
        class MockHttpMessageHandler : HttpMessageHandler
        {
            public Func<HttpRequestMessage, HttpResponseMessage> Callback { get; set; }

            public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> callback)
            {
                Callback = callback;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(Callback(request));
        }
        #endregion

        [TestMethod]
        public async Task SaveProtectedData_ShouldSucceed()
        {
            bool called = false;

            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                called = true;
                // TODO: Add foldermodel to content
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }
            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            ProtectedDataClient client = new ProtectedDataClient(clientFactory.Object);
            await client.Set("/Andys Passwords/MailAccount", "SecretPassword");

            Assert.IsTrue(called);
        }
    }
}
