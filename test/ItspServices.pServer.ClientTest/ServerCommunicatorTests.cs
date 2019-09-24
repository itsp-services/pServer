using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Client.Communicator;
using ItspServices.pServer.Client.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace ItspServices.pServer.ClientTest
{
    [TestClass]
    public class ServerCommunicatorTests
    {

        #region nested classes
        class MockHttpMessageHandler : HttpMessageHandler
        {
            public static Func<HttpRequestMessage, HttpResponseMessage> Callback;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Callback(request));
            }
        }

        class MockClientProvider : IClientProvider
        {
            public HttpClient GetClient()
            {
                HttpClient client = new HttpClient(new MockHttpMessageHandler());
                return client;
            }
        }
        #endregion

        [TestMethod]
        public void ServerCommunicator_RequestRootFolder_ShouldReturnRootFolder()
        {
            FolderModel rootFolder = new FolderModel()
            {
                ParentId = null,
                Name = "root",
                ProtectedDataIds = new List<int>(),
                SubfolderIds = new List<int>()
            };

            // Arrange
            ServerCommunicator communicator = new ServerCommunicator(new MockClientProvider());

            // Act
            FolderModel responseFolder = communicator.RequestFolderById(null);

            // Assert
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

            MockHttpMessageHandler.Callback = (request) =>
                new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new FormUrlEncodedContent(new[] 
                    {
                        new KeyValuePair<string, string>("ParentId", fooFolder.ParentId.ToString()),
                        new KeyValuePair<string, string>("Name", fooFolder.Name)
                    })
                };

            ServerCommunicator communicator = new ServerCommunicator(new MockClientProvider());

            FolderModel responseFolder = communicator.RequestFolderById(1);

            Assert.AreEqual(fooFolder.ParentId, responseFolder.ParentId);
            Assert.AreEqual(fooFolder.Name, responseFolder.Name);
            CollectionAssert.AreEquivalent(fooFolder.ProtectedDataIds.ToArray(), responseFolder.ProtectedDataIds.ToArray());
            CollectionAssert.AreEquivalent(fooFolder.SubfolderIds.ToArray(), responseFolder.SubfolderIds.ToArray());
        }
    }
}
