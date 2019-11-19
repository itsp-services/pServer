using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Client.Security;

namespace ItspServices.pServer.ClientTest
{
    [TestClass]
    public class DataEncryptorTests
    {
        [TestMethod]
        public void EncryptData_ShouldReturnEncryptedData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string encrypedData = dataEncryptor.EncryptData("SecretPassword", "key");
            Assert.AreNotEqual("SecretPassword", encrypedData);
        }

        [TestMethod]
        public void DecryptData_ShouldReturnDecryptedData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string decrypedData = dataEncryptor.DecryptData("EncrypedSecretPassword", "key");
            Assert.AreNotEqual("EncrypedSecretPassword", decrypedData);
        }

        [TestMethod]
        public void EncryptAndDecryptData_ShouldReturnCorrectData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string data = "SecretPassword";
            data = dataEncryptor.EncryptData(data, "key");
            data = dataEncryptor.DecryptData(data, "key");
            Assert.AreEqual("SecretPassword", data);
        }

        [TestMethod]
        public void EncryptAndDecryptData_ShouldReturnFalseData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string data = "SecretPassword";
            data = dataEncryptor.EncryptData(data, "key1");
            data = dataEncryptor.DecryptData(data, "key2");
            Assert.AreNotEqual("SecretPassword", data);
        }
    }
}
