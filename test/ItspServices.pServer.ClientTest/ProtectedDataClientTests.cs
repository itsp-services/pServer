using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ItspServices.pServer.Client;
using ItspServices.pServer.Client.Models;
using ItspServices.pServer.Client.RestApi;
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
            Mock<IApiClient> restClient = new Mock<IApiClient>();
            Mock<ILocalKeysController> localKeysController = new Mock<ILocalKeysController>();
            Mock<IDataEncryptor> dataEncryptor = new Mock<IDataEncryptor>();
            List<bool> wasCalled = new List<bool>();
            async Task<DataModel> CheckRequestDataByPath()
            {
                wasCalled.Add(true);
                return null;
            }
            async Task<int> CheckSendCreateData()
            {
                wasCalled.Add(true);
                return 1;
            }
            async Task CheckSendCreateKeyPairWithFileId()
            {
                wasCalled.Add(true);
                return;
            }

            restClient.Setup(x => x.RequestDataByPath(It.Is<string>(s => s == "AndysPasswords/MailAccount.data")))
                .Returns(CheckRequestDataByPath);

            restClient.Setup(x => x.SendCreateData(It.Is<string>(s => s == "AndysPasswords/MailAccount.data"), It.Is<DataModel>(s => s.Name == "MailAccount.data" && s.Data == Convert.ToBase64String(Encoding.Default.GetBytes("EncryptedSecretPassword")))))
                .Returns(CheckSendCreateData);

            restClient.Setup(x => x.SendCreateKeyPairWithFileId(It.Is<int>(s => s == 1), It.Is<KeyPairModel>(s => s.PublicKey == Convert.ToBase64String(Encoding.Default.GetBytes("publicKey")) && s.SymmetricKey == Convert.ToBase64String(Encoding.Default.GetBytes("publicSymmetricKey")))))
                .Returns(CheckSendCreateKeyPairWithFileId);

            localKeysController.Setup(x => x.GetPublicKey())
                .Returns(Convert.ToBase64String(Encoding.Default.GetBytes("publicKey")));

            dataEncryptor.Setup(x => x.CreateSymmetricKey(It.IsAny<int>()))
                .Returns(Encoding.Default.GetBytes("symmetricKey"));

            dataEncryptor.Setup(x => x.SymmetricEncryptData(It.Is<byte[]>(s => s.SequenceEqual(Encoding.Default.GetBytes("SecretPassword"))), It.Is<byte[]>(s => s.SequenceEqual(Encoding.Default.GetBytes("symmetricKey")))))
                .Returns(Encoding.Default.GetBytes("EncryptedSecretPassword"));

            dataEncryptor.Setup(x => x.AsymmetricEncryptData(It.Is<byte[]>(s => s.SequenceEqual(Encoding.Default.GetBytes("symmetricKey"))), It.Is<byte[]>(s => s.SequenceEqual(Encoding.Default.GetBytes("publicKey")))))
                .Returns(Encoding.Default.GetBytes("publicSymmetricKey"));

            ProtectedDataClient client = new ProtectedDataClient(localKeysController.Object);
            client.SetClient(restClient.Object);
            client.SetEncryptor(dataEncryptor.Object);
            await client.Set("AndysPasswords/MailAccount.data", "SecretPassword");

            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(wasCalled[i]);
            }
        }

        [TestMethod]
        public async Task UpdateProtectedData_ShouldSendEncryptedData()
        {
            Mock<IApiClient> restClient = new Mock<IApiClient>();
            Mock<ILocalKeysController> localKeysController = new Mock<ILocalKeysController>();
            Mock<IDataEncryptor> dataEncryptor = new Mock<IDataEncryptor>();
            List<bool> wasCalled = new List<bool>();
            async Task<DataModel> CheckRequestDataByPath()
            {
                wasCalled.Add(true);
                return new DataModel
                {
                    Name = "MailAccount.data",
                    Data = "OldData"
                };
            }
            async Task<KeyPairModel[]> CheckRequestKeyPairsByFilePath()
            {
                wasCalled.Add(true);
                return new KeyPairModel[]
                {
                    new KeyPairModel()
                    {
                        PublicKey = "publicKey",
                        SymmetricKey = "publicSymmetricKey"
                    },
                    new KeyPairModel()
                    {
                        PublicKey = Convert.ToBase64String(Encoding.Default.GetBytes("wrongPublicKey")),
                        SymmetricKey = Convert.ToBase64String(Encoding.Default.GetBytes("wrongPublicSymmetricKey"))
                    },
                    new KeyPairModel()
                    {
                        PublicKey = Convert.ToBase64String(Encoding.Default.GetBytes("publicKey")),
                        SymmetricKey = Convert.ToBase64String(Encoding.Default.GetBytes("publicSymmetricKey"))
                    }
                };
            }
            async Task CheckSendUpdateData()
            {
                wasCalled.Add(true);
                return;
            }

            restClient.Setup(x => x.RequestDataByPath(It.Is<string>(s => s == "AndysPasswords/MailAccount.data")))
                .Returns(CheckRequestDataByPath);

            restClient.Setup(x => x.RequestKeyPairsByFilePath(It.Is<string>(s => s == "AndysPasswords/MailAccount.data")))
                .Returns(CheckRequestKeyPairsByFilePath);

            restClient.Setup(x => x.SendUpdateData(It.Is<string>(s => s == "AndysPasswords/MailAccount.data"), It.Is<DataModel>(s => s.Name == "MailAccount.data" && s.Data == Convert.ToBase64String(Encoding.Default.GetBytes("EncryptedSecretPassword")))))
                .Returns(CheckSendUpdateData);

            localKeysController.Setup(x => x.GetPublicKey())
                .Returns(Convert.ToBase64String(Encoding.Default.GetBytes("publicKey")));

            localKeysController.Setup(x => x.GetPrivateKey())
                .Returns(Convert.ToBase64String(Encoding.Default.GetBytes("privateKey")));

            dataEncryptor.Setup(x => x.SymmetricEncryptData(It.Is<byte[]>(s => s.SequenceEqual(Encoding.Default.GetBytes("SecretPassword"))), It.Is<byte[]>(s => s.SequenceEqual(Encoding.Default.GetBytes("symmetricKey")))))
                .Returns(Encoding.Default.GetBytes("EncryptedSecretPassword"));

            dataEncryptor.Setup(x => x.AsymmetricDecryptData(It.Is<byte[]>(s => s.SequenceEqual(Encoding.Default.GetBytes("publicSymmetricKey"))), It.Is<byte[]>(s => s.SequenceEqual(Encoding.Default.GetBytes("privateKey")))))
                .Returns(Encoding.Default.GetBytes("symmetricKey"));




            ProtectedDataClient client = new ProtectedDataClient(localKeysController.Object);
            client.SetClient(restClient.Object);
            client.SetEncryptor(dataEncryptor.Object);
            await client.Set("AndysPasswords/MailAccount.data", "SecretPassword");

            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(wasCalled[i]);
            }
        }
    }
}
