using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ItspServices.pServer.Client;
using ItspServices.pServer.Client.Security;
using System.Text.Json;
using ItspServices.pServer.Client.Models;

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
        public async Task CreateProtectedData_ShouldSucceed()
        {
            bool setHasBeenCalled = false;

            Mock<IDataEncryptor> dataEncryptor = new Mock<IDataEncryptor>();
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                string content = null;
                switch (request.RequestUri.AbsolutePath)
                {
                    case "/api/protecteddata/folder/":
                        content = "{\"ParentId\":null,\"Name\":\"root\",\"ProtectedDataIds\":[],\"SubfolderIds\":[1,2,3]}";
                        break;
                    case "/api/protecteddata/folder/1":
                        content = "{\"ParentId\":null,\"Name\":\"FirstFolder\",\"ProtectedDataIds\":[],\"SubfolderIds\":[]}";
                        break;
                    case "/api/protecteddata/folder/2":
                        content = "{\"ParentId\":null,\"Name\":\"Andys Passwords\",\"ProtectedDataIds\":[],\"SubfolderIds\":[4,5,6]}";
                        break;
                    case "/api/protecteddata/folder/3":
                    case "/api/protecteddata/folder/4":
                    case "/api/protecteddata/folder/5":
                    case "/api/protecteddata/folder/6":
                        Assert.Fail($"Client doesn't need to ask for this folder: {request.RequestUri.AbsolutePath}");
                        break;
                    case var s when s == "/api/protecteddata/data/1" && request.Method == HttpMethod.Get:
                        content = "{\"Name\":\"MailAccount\",\"Data\":\"SecretPassword\"}";
                        break;
                    case var s when s == "/api/protecteddata/data/2" && request.Method == HttpMethod.Post:
                        setHasBeenCalled = true;
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
                    default:
                        Assert.Fail("Invalid requested URI");
                        break;
                }

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(content)
                };
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            ProtectedDataClient client = new ProtectedDataClient(clientFactory.Object, dataEncryptor.Object);
            await client.Set("/Andys Passwords/MailAccount", "SecretPassword");

            Assert.IsTrue(setHasBeenCalled);
        }

        [TestMethod]
        public async Task UpdateProtectedData_ShouldSucceed()
        {
            bool setHasBeenCalled = false;

            Mock<IDataEncryptor> dataEncryptor = new Mock<IDataEncryptor>();
            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                string content = null;
                switch (request.RequestUri.AbsolutePath)
                {
                    case "/api/protecteddata/folder/":
                        content = "{\"ParentId\":null,\"Name\":\"root\",\"ProtectedDataIds\":[],\"SubfolderIds\":[1,2,3]}";
                        break;
                    case "/api/protecteddata/folder/1":
                        content = "{\"ParentId\":null,\"Name\":\"FirstFolder\",\"ProtectedDataIds\":[],\"SubfolderIds\":[]}";
                        break;
                    case "/api/protecteddata/folder/2":
                        content = "{\"ParentId\":null,\"Name\":\"Andys Passwords\",\"ProtectedDataIds\":[1],\"SubfolderIds\":[4,5,6]}";
                        break;
                    case "/api/protecteddata/folder/3":
                    case "/api/protecteddata/folder/4":
                    case "/api/protecteddata/folder/5":
                    case "/api/protecteddata/folder/6":
                        Assert.Fail($"Client doesn't need to ask for this folder: {request.RequestUri.AbsolutePath}");
                        break;
                    case var s when s == "/api/protecteddata/data/1" && request.Method == HttpMethod.Get:
                        content = "{\"Name\":\"MailAccount\",\"Data\":\"SecretPassword\"}";
                        break;
                    case var s when s == "/api/protecteddata/data/1" && request.Method == HttpMethod.Put:
                        setHasBeenCalled = true;
                        return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                    default:
                        Assert.Fail("Invalid requested URI");
                        break;
                }

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(content)
                };
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            ProtectedDataClient client = new ProtectedDataClient(clientFactory.Object, dataEncryptor.Object);
            await client.Set("/Andys Passwords/MailAccount", "SecretPassword");

            Assert.IsTrue(setHasBeenCalled);
        }

        [TestMethod]
        public async Task CreateProtectedData_ShouldEncrypt()
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            Mock<IDataEncryptor> dataEncryptor = new Mock<IDataEncryptor>();

            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                string content = null;
                switch (request.RequestUri.AbsolutePath)
                {
                    case "/api/protecteddata/folder/":
                        content = "{\"ParentId\":null,\"Name\":\"root\",\"ProtectedDataIds\":[],\"SubfolderIds\":[1,2,3]}";
                        break;
                    case "/api/protecteddata/folder/1":
                        content = "{\"ParentId\":null,\"Name\":\"FirstFolder\",\"ProtectedDataIds\":[],\"SubfolderIds\":[]}";
                        break;
                    case "/api/protecteddata/folder/2":
                        content = "{\"ParentId\":null,\"Name\":\"Andys Passwords\",\"ProtectedDataIds\":[],\"SubfolderIds\":[4,5,6]}";
                        break;
                    case "/api/protecteddata/folder/3":
                    case "/api/protecteddata/folder/4":
                    case "/api/protecteddata/folder/5":
                    case "/api/protecteddata/folder/6":
                        Assert.Fail($"Client doesn't need to ask for this folder: {request.RequestUri.AbsolutePath}");
                        break;
                    case var s when s == "/api/protecteddata/data/1" && request.Method == HttpMethod.Get:
                        content = "{\"Name\":\"MailAccount\",\"Data\":\"SecretPassword\"}";
                        break;
                    case var s when s == "/api/protecteddata/data/2" && request.Method == HttpMethod.Post:
                        requestMessage = request;
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
                    default:
                        Assert.Fail("Invalid requested URI");
                        break;
                }

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(content)
                };
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            dataEncryptor.Setup(x => x.EncryptWithSymmetricKey(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(() => Encoding.Unicode.GetBytes("EncryptedPassword"));

            ProtectedDataClient client = new ProtectedDataClient(clientFactory.Object, dataEncryptor.Object);
            await client.Set("/Andys Passwords/MailAccount", "SecretPassword");

            Assert.AreEqual("EncryptedPassword", (await JsonSerializer.DeserializeAsync<DataModel>(await requestMessage.Content.ReadAsStreamAsync())).Data);
        }

        [TestMethod]
        public async Task UpdateProtectedData_ShouldEncrypt()
        {

            HttpRequestMessage requestMessage = new HttpRequestMessage();

            Mock<IDataEncryptor> dataEncryptor = new Mock<IDataEncryptor>();

            Mock<IHttpClientFactory> clientFactory = new Mock<IHttpClientFactory>();
            HttpResponseMessage Callback(HttpRequestMessage request)
            {
                string content = null;
                switch (request.RequestUri.AbsolutePath)
                {
                    case "/api/protecteddata/folder/":
                        content = "{\"ParentId\":null,\"Name\":\"root\",\"ProtectedDataIds\":[],\"SubfolderIds\":[1,2,3]}";
                        break;
                    case "/api/protecteddata/folder/1":
                        content = "{\"ParentId\":null,\"Name\":\"FirstFolder\",\"ProtectedDataIds\":[],\"SubfolderIds\":[]}";
                        break;
                    case "/api/protecteddata/folder/2":
                        content = "{\"ParentId\":null,\"Name\":\"Andys Passwords\",\"ProtectedDataIds\":[1],\"SubfolderIds\":[4,5,6]}";
                        break;
                    case "/api/protecteddata/folder/3":
                    case "/api/protecteddata/folder/4":
                    case "/api/protecteddata/folder/5":
                    case "/api/protecteddata/folder/6":
                        Assert.Fail($"Client doesn't need to ask for this folder: {request.RequestUri.AbsolutePath}");
                        break;
                    case var s when s == "/api/protecteddata/data/1" && request.Method == HttpMethod.Get:
                        content = "{\"Name\":\"MailAccount\",\"Data\":\"SecretPassword\"}";
                        break;
                    case var s when s == "/api/protecteddata/data/1" && request.Method == HttpMethod.Put:
                        requestMessage = request;
                        return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                    default:
                        Assert.Fail("Invalid requested URI");
                        break;
                }

                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(content)
                };
            }

            clientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                    new HttpClient(new MockHttpMessageHandler(Callback))
                    {
                        BaseAddress = new Uri("http://test.com")
                    });

            dataEncryptor.Setup(x => x.EncryptWithSymmetricKey(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(() => Encoding.Unicode.GetBytes("EncryptedPassword"));

            ProtectedDataClient client = new ProtectedDataClient(clientFactory.Object, dataEncryptor.Object);
            await client.Set("/Andys Passwords/MailAccount", "SecretPassword");

            Assert.AreEqual("EncryptedPassword", (await JsonSerializer.DeserializeAsync<DataModel>(await requestMessage.Content.ReadAsStreamAsync())).Data);
            
        }
    }
}
