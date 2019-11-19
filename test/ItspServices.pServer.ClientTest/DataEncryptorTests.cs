using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Client.Security;

namespace ItspServices.pServer.ClientTest
{
    [TestClass]
    public class DataEncryptorTests
    {
        [TestMethod]
        public void SymmetricEncryptData_ShouldReturnEncryptedData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string encrypedData = dataEncryptor.SymmetricEncryptData("SecretPassword", "symmetricKey");
            Assert.AreNotEqual("SecretPassword", encrypedData);
        }

        [TestMethod]
        public void SymmetricDecryptData_ShouldReturnDecryptedData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string decrypedData = dataEncryptor.SymmetricDecryptData("EncrypedSecretPassword", "symmetricKey");
            Assert.AreNotEqual("EncrypedSecretPassword", decrypedData);
        }

        [TestMethod]
        public void SymmetricEncryptAndDecryptData_ShouldReturnCorrectData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string data = "SecretPassword";
            data = dataEncryptor.SymmetricEncryptData(data, "symmetricKey");
            data = dataEncryptor.SymmetricDecryptData(data, "symmetricKey");
            Assert.AreEqual("SecretPassword", data);
        }

        [TestMethod]
        public void SymmetricEncryptAndDecryptData_ShouldReturnFalseData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string data = "SecretPassword";
            data = dataEncryptor.SymmetricEncryptData(data, "symmetricKey1");
            data = dataEncryptor.SymmetricDecryptData(data, "symmetricKey2");
            Assert.AreNotEqual("SecretPassword", data);
        }

        [TestMethod]
        public void AsymmetricEncryptData_ShouldReturnEncryptedData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string encrypedData = dataEncryptor.AsymmetricEncryptData("SecretPassword", "publicKey");
            Assert.AreNotEqual("SecretPassword", encrypedData);
        }

        [TestMethod]
        public void AsymmetricDecryptData_ShouldReturnDecryptedData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string decrypedData = dataEncryptor.AsymmetricDecryptData("EncrypedSecretPassword", "privatecKey");
            Assert.AreNotEqual("EncrypedSecretPassword", decrypedData);
        }

        [TestMethod]
        public void AsymmetricEncryptAndDecryptData_ShouldReturnCorrectData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string data = "SecretPassword";
            data = dataEncryptor.AsymmetricEncryptData(data, "publicKey");
            data = dataEncryptor.AsymmetricDecryptData(data, "privateKey");
            Assert.AreEqual("SecretPassword", data);
        }

        [TestMethod]
        public void AsymmetricEncryptAndDecryptData_ShouldReturnFalseData()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            string data = "SecretPassword";
            data = dataEncryptor.AsymmetricEncryptData(data, "publicKey");
            data = dataEncryptor.AsymmetricDecryptData(data, "publicKey");
            Assert.AreNotEqual("SecretPassword", data);
        }
    }
}
