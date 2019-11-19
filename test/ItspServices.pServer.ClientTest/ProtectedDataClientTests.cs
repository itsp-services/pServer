using System;
using System.Collections.Generic;
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

            restClient.Setup(x => x.SendCreateData(It.Is<string>(s => s == "AndysPasswords/MailAccount.data"), It.Is<DataModel>(s => s.Name == "MailAccount.data" && s.Data == "EncryptedSecretPassword")))
                .Returns(CheckSendCreateData);

            restClient.Setup(x => x.SendCreateKeyPairWithFileId(It.Is<int>(s => s == 1), It.Is<KeyPairModel>(s => s.PublicKey == "publicKey" && s.SymmetricKey == "publicSymmetricKey")))
                .Returns(CheckSendCreateKeyPairWithFileId);

            localKeysController.Setup(x => x.GetPublicKey())
                .Returns("publicKey");

            localKeysController.Setup(x => x.CreateSymmetricKey())
                .Returns("symmetricKey");

            dataEncryptor.Setup(x => x.AsymmetricEncryptData(It.Is<string>(s => s == "symmetricKey"), It.Is<string>(s => s == "publicKey")))
                .Returns("publicSymmetricKey");

            dataEncryptor.Setup(x => x.SymmetricEncryptData(It.Is<string>(s => s == "SecretPassword"), It.Is<string>(s => s == "symmetricKey")))
                .Returns("EncryptedSecretPassword");

            ProtectedDataClient client = new ProtectedDataClient(restClient.Object, localKeysController.Object, dataEncryptor.Object);
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

            restClient.Setup(x => x.SendUpdateData(It.Is<string>(s => s == "AndysPasswords/MailAccount.data"), It.Is<DataModel>(s => s.Name == "MailAccount.data" && s.Data == "EncryptedSecretPassword")))
                .Returns(CheckSendUpdateData);

            localKeysController.Setup(x => x.GetPublicKey())
                .Returns("publicKey");

            localKeysController.Setup(x => x.GetPrivateKey())
                .Returns("privateKey");

            dataEncryptor.Setup(x => x.AsymmetricDecryptData(It.Is<string>(s => s == "publicSymmetricKey"), It.Is<string>(s => s == "privateKey")))
                .Returns("symmetricKey");

            dataEncryptor.Setup(x => x.SymmetricEncryptData(It.Is<string>(s => s == "SecretPassword"), It.Is<string>(s => s == "symmetricKey")))
                .Returns("EncryptedSecretPassword");

            ProtectedDataClient client = new ProtectedDataClient(restClient.Object, localKeysController.Object, dataEncryptor.Object);
            await client.Set("AndysPasswords/MailAccount.data", "SecretPassword");

            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(wasCalled[i]);
            }
        }
    }
}
