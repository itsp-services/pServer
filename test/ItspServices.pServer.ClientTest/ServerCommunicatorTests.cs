using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Client.Communicator;
using ItspServices.pServer.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using Moq;

namespace ItspServices.pServer.ClientTest
{
    [TestClass]
    public class ServerCommunicatorTests
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
            {
                return Task.FromResult(Callback(request));
            }
        }
        #endregion

        [TestMethod]
        public async Task ServerCommunicator_RequestRootFolder_ShouldReturnRootFolder()
        {
            FolderModel rootFolder = new FolderModel()
            {
                ParentId = null,
                Name = "root",
                ProtectedDataIds = new List<int>(),
                SubfolderIds = new List<int>()
            };
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            Func<HttpRequestMessage, HttpResponseMessage> callback = (request) =>
            {
                Assert.AreEqual("/api/protecteddata/folder/", request.RequestUri.LocalPath);
                string json = JsonSerializer.Serialize(rootFolder);
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(json) };
            };
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new MockHttpMessageHandler(callback)) { BaseAddress = new Uri("http://test.com") });

            ServerCommunicator communicator = new ServerCommunicator(clientFactory.Object);

            FolderModel responseFolder = await communicator.RequestFolderById(null);

            Assert.AreEqual(rootFolder.ParentId, responseFolder.ParentId);
            Assert.AreEqual(rootFolder.Name, responseFolder.Name);
            CollectionAssert.AreEquivalent(rootFolder.ProtectedDataIds.ToArray(), responseFolder.ProtectedDataIds.ToArray());
            CollectionAssert.AreEquivalent(rootFolder.SubfolderIds.ToArray(), responseFolder.SubfolderIds.ToArray());
        }

        [TestMethod]
        public async Task ServerCommunicator_RequestAnyFolderById_ShouldReturnCorrectFolder()
        {
            FolderModel fooFolder = new FolderModel()
            {
                ParentId = 99,
                Name = "foo",
                ProtectedDataIds = new int[] { 1, 2, 3 }.ToList(),
                SubfolderIds = new int[] { 4, 5, 6 }.ToList()
            };
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            Func<HttpRequestMessage, HttpResponseMessage> callback = (request) =>
            {
                Assert.AreEqual("/api/protecteddata/folder/1", request.RequestUri.LocalPath);
                string json = JsonSerializer.Serialize(fooFolder);
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(json) };
            };
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new MockHttpMessageHandler(callback)) { BaseAddress = new Uri("http://test.com") });

            ServerCommunicator communicator = new ServerCommunicator(clientFactory.Object);
            FolderModel responseFolder = await communicator.RequestFolderById(1);

            Assert.AreEqual(fooFolder.ParentId, responseFolder.ParentId);
            Assert.AreEqual(fooFolder.Name, responseFolder.Name);
            CollectionAssert.AreEquivalent(fooFolder.ProtectedDataIds.ToArray(), responseFolder.ProtectedDataIds.ToArray());
            CollectionAssert.AreEquivalent(fooFolder.SubfolderIds.ToArray(), responseFolder.SubfolderIds.ToArray());
        }
    }
}
