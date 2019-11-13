using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ItspServices.pServer.Client;
using ItspServices.pServer.Client.Models;
using ItspServices.pServer.Client.Security;

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
        public async Task CreateProtectedData_ShouldSendEncryptedDataAndKeyPair()
        {
            List<HttpRequestMessage> requestMessages = new List<HttpRequestMessage>();
            Mock<IDataEncryptor> dataEncryptor = new Mock<IDataEncryptor>();
            Mock<ILocalKeysController> localKeysController = new Mock<ILocalKeysController>();
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                switch (request.RequestUri.AbsolutePath)
                {
                    case var s when s == "/api/protecteddata/data/AndysPasswords/MailAccount.data" && request.Method == HttpMethod.Get:
                        return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                    case var s when s == "/api/protecteddata/data/" && request.Method == HttpMethod.Post:
                        requestMessages.Add(request);
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Created)
                        {
                            Content = new StringContent("1")
                        };
                    case var s when s == "/api/protecteddata/key/1" && request.Method == HttpMethod.Post:
                        requestMessages.Add(request);
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
                    default:
                        Assert.Fail("Invalid requested URI");
                        break;
                }
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            dataEncryptor.Setup(x => x.EncryptData(It.Is<string>(s => s == "symmetricKey"), It.Is<string>(s => s == "publicKey")))
                .Returns("publicSymmetricKey");

            dataEncryptor.Setup(x => x.EncryptData(It.Is<string>(s => s == "SecretPassword"), It.Is<string>(s => s == "symmetricKey")))
                .Returns("EncryptedSecretPassword");

            localKeysController.Setup(x => x.GetPublicKey())
                .Returns("publicKey");

            localKeysController.Setup(x => x.CreateSymmetricKey())
                .Returns("symmetricKey");

            ProtectedDataClient client = new ProtectedDataClient(clientFactory.Object, localKeysController.Object, dataEncryptor.Object);
            await client.Set("AndysPasswords/MailAccount.data", "SecretPassword");

            DataModelWithPath dataModelWithPath = await JsonSerializer.DeserializeAsync<DataModelWithPath>(await requestMessages[0].Content.ReadAsStreamAsync());

            Assert.AreEqual("AndysPasswords/MailAccount.data", dataModelWithPath.Path);
            Assert.AreEqual("MailAccount.data", dataModelWithPath.DataModel.Name);
            Assert.AreEqual("EncryptedSecretPassword", dataModelWithPath.DataModel.Data);

            KeyPairModel keyPairModel = await JsonSerializer.DeserializeAsync<KeyPairModel>(await requestMessages[1].Content.ReadAsStreamAsync());

            Assert.AreEqual("publicKey", keyPairModel.PublicKey);
            Assert.AreEqual("publicSymmetricKey", keyPairModel.SymmetricKey);
        }

        [TestMethod]
        public async Task UpdateProtectedData_ShouldSendEncryptedData()
        {
            List<HttpRequestMessage> requestMessages = new List<HttpRequestMessage>();
            Mock<IDataEncryptor> dataEncryptor = new Mock<IDataEncryptor>();
            Mock<ILocalKeysController> localKeysController = new Mock<ILocalKeysController>();
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                switch (request.RequestUri.AbsolutePath)
                {
                    case var s when s == "/api/protecteddata/data/AndysPasswords/MailAccount.data" && request.Method == HttpMethod.Get:
                        return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent(JsonSerializer.Serialize(new DataModel
                            {
                                Name = "MailAccount.data",
                                Data = "OldData"
                            }))
                        };
                    case var s when s == "/api/protecteddata/key/AndysPasswords/MailAccount.data" && request.Method == HttpMethod.Get:
                        return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent(JsonSerializer.Serialize(new KeyPairModel[]
                            {
                                new KeyPairModel()
                                {
                                    PublicKey = "publicKey1",
                                    SymmetricKey = "publicSymmetricKey1"
                                },
                                new KeyPairModel()
                                {
                                    PublicKey = "publicKey2",
                                    SymmetricKey = "publicSymmetricKey2"
                                },
                                new KeyPairModel()
                                {
                                    PublicKey = "publicKey",
                                    SymmetricKey = "publicSymmetricKey"
                                }
                            }))
                        };
                    case var s when s == "/api/protecteddata/data/AndysPasswords/MailAccount.data" && request.Method == HttpMethod.Put:
                        requestMessages.Add(request);
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
                    default:
                        Assert.Fail("Invalid requested URI");
                        break;
                }
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            dataEncryptor.Setup(x => x.DecryptData(It.Is<string>(s => s == "publicSymmetricKey"), It.Is<string>(s => s == "privateKey")))
                .Returns("symmetricKey");

            dataEncryptor.Setup(x => x.EncryptData(It.Is<string>(s => s == "SecretPassword"), It.Is<string>(s => s == "symmetricKey")))
                .Returns("EncryptedSecretPassword");

            localKeysController.Setup(x => x.GetPublicKey())
                .Returns("publicKey");

            localKeysController.Setup(x => x.GetPrivateKey())
                .Returns("privateKey");

            ProtectedDataClient client = new ProtectedDataClient(clientFactory.Object, localKeysController.Object, dataEncryptor.Object);
            await client.Set("AndysPasswords/MailAccount.data", "SecretPassword");

            DataModel dataModel = await JsonSerializer.DeserializeAsync<DataModel>(await requestMessages[0].Content.ReadAsStreamAsync());

            Assert.AreEqual("MailAccount.data", dataModel.Name);
            Assert.AreEqual("EncryptedSecretPassword", dataModel.Data);
        }
    }
}
