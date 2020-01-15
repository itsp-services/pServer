using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ItspServices.pServer.Client.RestApi;
using ItspServices.pServer.Client.Models;
using ItspServices.pServer.Client.Datatypes;

namespace ItspServices.pServer.ClientTest
{
    [TestClass]
    public class RestApiClientTests
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
        public async Task RequestRootFolder_ShouldReturnRootFolder()
        {
            FolderModel rootFolder = new FolderModel()
            {
                ParentId = null,
                Name = "root",
                ProtectedDataIds = new List<int>(),
                SubfolderIds = new List<int>()
            };
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                Assert.AreEqual("/api/protecteddata/folder/", request.RequestUri.LocalPath);
                string json = JsonSerializer.Serialize(rootFolder);
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(json) };
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            IApiClient restClient = new RestApiClient(clientFactory.Object);
            FolderModel responseFolder = await restClient.RequestFolderById(null);

            Assert.AreEqual(rootFolder.ParentId, responseFolder.ParentId);
            Assert.AreEqual(rootFolder.Name, responseFolder.Name);
            CollectionAssert.AreEquivalent(rootFolder.ProtectedDataIds.ToArray(), responseFolder.ProtectedDataIds.ToArray());
            CollectionAssert.AreEquivalent(rootFolder.SubfolderIds.ToArray(), responseFolder.SubfolderIds.ToArray());
        }

        [TestMethod]
        public async Task RequestAnyFolderById_ShouldReturnCorrectFolder()
        {
            FolderModel fooFolder = new FolderModel()
            {
                ParentId = 99,
                Name = "foo",
                ProtectedDataIds = new int[] { 1, 2, 3 }.ToList(),
                SubfolderIds = new int[] { 4, 5, 6 }.ToList()
            };
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                Assert.AreEqual("/api/protecteddata/folder/1", request.RequestUri.LocalPath);
                string json = JsonSerializer.Serialize(fooFolder);
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(json) };
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            IApiClient restClient = new RestApiClient(clientFactory.Object);
            FolderModel responseFolder = await restClient.RequestFolderById(1);

            Assert.AreEqual(fooFolder.ParentId, responseFolder.ParentId);
            Assert.AreEqual(fooFolder.Name, responseFolder.Name);
            CollectionAssert.AreEquivalent(fooFolder.ProtectedDataIds.ToArray(), responseFolder.ProtectedDataIds.ToArray());
            CollectionAssert.AreEquivalent(fooFolder.SubfolderIds.ToArray(), responseFolder.SubfolderIds.ToArray());
        }

        [TestMethod]
        public async Task RequestDataByPath_ShouldReturnCorrectDataModel()
        {
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                Assert.AreEqual("/api/protecteddata/data/AndysPasswords/MailAccount.data", request.RequestUri.LocalPath);
                Assert.AreEqual(HttpMethod.Get, request.Method);
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new DataModel
                    {
                        Name = "MailAccount.data",
                        Data = "SecretPassword"
                    }))
                };
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            IApiClient restClient = new RestApiClient(clientFactory.Object);
            DataModel dataModel = await restClient.RequestDataByPath("AndysPasswords/MailAccount.data");

            Assert.AreEqual("MailAccount.data", dataModel.Name);
            Assert.AreEqual("SecretPassword", dataModel.Data);
        }


        [TestMethod]
        public async Task RequestNullData_ShouldReturnNull()
        {
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                Assert.AreEqual("/api/protecteddata/data/AndysPasswords/MailAccount.data", request.RequestUri.LocalPath);
                Assert.AreEqual(HttpMethod.Get, request.Method);
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            IApiClient restClient = new RestApiClient(clientFactory.Object);
            DataModel dataModel = await restClient.RequestDataByPath("AndysPasswords/MailAccount.data");

            Assert.IsNull(dataModel);
        }

        [TestMethod]
        public async Task SendCreateData_ShouldSendDataAndReturnId()
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                Assert.AreEqual("/api/protecteddata/data/", request.RequestUri.LocalPath);
                Assert.AreEqual(HttpMethod.Post, request.Method);
                requestMessage = request;
                return new HttpResponseMessage(System.Net.HttpStatusCode.Created)
                {
                    Content = new StringContent("1")
                };
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            IApiClient restClient = new RestApiClient(clientFactory.Object);
            int id = await restClient.SendCreateData("AndysPasswords/MailAccount.data", new DataModel
            {
                Name = "MailAccount.data",
                Data = "SecretPassword"
            });
            DataModelWithPath dataModel = await JsonSerializer.DeserializeAsync<DataModelWithPath>(await requestMessage.Content.ReadAsStreamAsync());

            Assert.AreEqual("MailAccount.data", dataModel.DataModel.Name);
            Assert.AreEqual("SecretPassword", dataModel.DataModel.Data);
            Assert.AreEqual("AndysPasswords/MailAccount.data", dataModel.Path);
            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public async Task SendUpdateData_ShouldSendData()
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                Assert.AreEqual("/api/protecteddata/data/AndysPasswords/MailAccount.data", request.RequestUri.LocalPath);
                Assert.AreEqual(HttpMethod.Put, request.Method);
                requestMessage = request;
                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            IApiClient restClient = new RestApiClient(clientFactory.Object);
            await restClient.SendUpdateData("AndysPasswords/MailAccount.data", new DataModel
            {
                Name = "MailAccount.data",
                Data = "SecretPassword"
            });
            DataModel dataModel = await JsonSerializer.DeserializeAsync<DataModel>(await requestMessage.Content.ReadAsStreamAsync());

            Assert.AreEqual("MailAccount.data", dataModel.Name);
            Assert.AreEqual("SecretPassword", dataModel.Data);
        }

        [TestMethod]
        public async Task RequestKeyPairsByFilePath_ShouldReturnAuthorizedKeyPairs()
        {
            KeyPairModel[] expectedKeyPairModels = new KeyPairModel[]
            {
                new KeyPairModel()
                {
                    PublicKey = Convert.ToBase64String(Encoding.Default.GetBytes("publicKey1")),
                    SymmetricKey = Convert.ToBase64String(Encoding.Default.GetBytes("symmetricKey1"))
                },
                new KeyPairModel()
                {
                    PublicKey = Convert.ToBase64String(Encoding.Default.GetBytes("publicKey2")),
                    SymmetricKey = Convert.ToBase64String(Encoding.Default.GetBytes("symmetricKey2"))
                }
            };
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                Assert.AreEqual("/api/protecteddata/key/AndysPasswords/MailAccount.data", request.RequestUri.LocalPath);
                Assert.AreEqual(HttpMethod.Get, request.Method);
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(expectedKeyPairModels))
                };
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            IApiClient restClient = new RestApiClient(clientFactory.Object);
            KeyPair[] keyPairs = await restClient.RequestKeyPairsByFilePath("AndysPasswords/MailAccount.data");

            Assert.AreEqual(expectedKeyPairModels.Length, keyPairs.Length);
            for (int i = 0; i < keyPairs.Length; i++)
            {
                Assert.AreEqual(expectedKeyPairModels[i].PublicKey, keyPairs[i].PublicKey.GetBase64());
                Assert.AreEqual(expectedKeyPairModels[i].SymmetricKey, keyPairs[i].SymmetricKey.GetBase64());
            }
            // ?? couldnt find an array comparing function
        }

        [TestMethod]
        public async Task SendCreateKeyPairWithFileId_ShouldSendKeyPair()
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                Assert.AreEqual("/api/protecteddata/key/1", request.RequestUri.LocalPath);
                Assert.AreEqual(HttpMethod.Post, request.Method);
                requestMessage = request;
                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            IApiClient restClient = new RestApiClient(clientFactory.Object);
            await restClient.SendCreateKeyPairWithFileId(1, new KeyPair
            {
                PublicKey = Encoding.Default.GetBytes("publicKey"),
                SymmetricKey = Encoding.Default.GetBytes("symmetricKey")
            });
            KeyPairModel keyPairModel = await JsonSerializer.DeserializeAsync<KeyPairModel>(await requestMessage.Content.ReadAsStreamAsync());

            Assert.AreEqual(Convert.ToBase64String(Encoding.Default.GetBytes("publicKey")), keyPairModel.PublicKey);
            Assert.AreEqual(Convert.ToBase64String(Encoding.Default.GetBytes("symmetricKey")), keyPairModel.SymmetricKey);
        }
    }
}
