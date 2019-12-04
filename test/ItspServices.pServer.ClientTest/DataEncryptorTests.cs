using System;
using System.Text;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Client.Security;

namespace ItspServices.pServer.ClientTest
{
    [TestClass]
    public class DataEncryptorTests
    {
        [TestMethod]
        public void CreateSymmetricKey_ShouldReturnValidKey()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            byte[] symmetricKey;
            int ivLength = 128;

            symmetricKey = dataEncryptor.CreateSymmetricKey();
            Assert.IsNotNull(symmetricKey);
            Assert.AreEqual(ivLength / 8 + 128 / 8, symmetricKey.Length);

            symmetricKey = dataEncryptor.CreateSymmetricKey(256);
            Assert.IsNotNull(symmetricKey);
            Assert.AreEqual(ivLength / 8 + 256 / 8, symmetricKey.Length);

            Assert.ThrowsException<ArgumentException>(() => symmetricKey = dataEncryptor.CreateSymmetricKey(333));
        }

        [TestMethod]
        public void CreateTwoSymmetricKeys_ShouldReturnDifferentKeys()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();

            byte[] symmetricKey = dataEncryptor.CreateSymmetricKey();
            byte[] differentSymmetricKey = dataEncryptor.CreateSymmetricKey();

            Assert.AreNotEqual(symmetricKey, differentSymmetricKey);
        }

        [TestMethod]
        public void SymmetricEncryptAndDecryptData_ShouldReturnCorrectData()
        {
            string expectedData = "SecretPassword";
            DataEncryptor dataEncryptor = new DataEncryptor();
            byte[] key = dataEncryptor.CreateSymmetricKey();

            byte[] encrypedData = dataEncryptor.SymmetricEncryptData(Encoding.Default.GetBytes(expectedData), key);
            byte[] decrypedData = dataEncryptor.SymmetricDecryptData(encrypedData, key);

            Assert.AreEqual(expectedData, Encoding.Default.GetString(decrypedData));
        }

        [TestMethod]
        public void SymmetricEncryptAndDecryptDataWithWrongKey_ShouldThrowException()
        {
            string expectedData = "SecretPassword";
            DataEncryptor dataEncryptor = new DataEncryptor();
            byte[] key = dataEncryptor.CreateSymmetricKey();

            byte[] encrypedData = dataEncryptor.SymmetricEncryptData(Encoding.Default.GetBytes(expectedData), key);
            byte[] differentKey = dataEncryptor.CreateSymmetricKey();

            Assert.ThrowsException<ArgumentException>(() => dataEncryptor.SymmetricDecryptData(encrypedData, differentKey));
        }

        [TestMethod]
        public void AsymmetricEncryptAndDecryptData_ShouldReturnCorrectData()
        {
            string expectedData = "SecretPassword";
            DataEncryptor dataEncryptor = new DataEncryptor();
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            byte[] publicKey = rsaProvider.ExportCspBlob(false);
            byte[] privateKey = rsaProvider.ExportCspBlob(true);

            byte[] encrypedData = dataEncryptor.AsymmetricEncryptData(Encoding.Default.GetBytes(expectedData), publicKey);
            byte[] decrypedData = dataEncryptor.AsymmetricDecryptData(encrypedData, privateKey);

            Assert.AreEqual(expectedData, Encoding.Default.GetString(decrypedData));
        }

        [TestMethod]
        public void AsymmetricEncryptAndDecryptDataWithWrongKey_ShouldThrowException()
        {
            string expectedData = "SecretPassword";
            DataEncryptor dataEncryptor = new DataEncryptor();
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            byte[] publicKey = rsaProvider.ExportCspBlob(false);
            byte[] privateKey = rsaProvider.ExportCspBlob(true);
            byte[] encrypedData;

            RSACryptoServiceProvider differentRsaProvider = new RSACryptoServiceProvider();
            byte[] differentPrivateKey = differentRsaProvider.ExportCspBlob(true);
            encrypedData = dataEncryptor.AsymmetricEncryptData(Encoding.Default.GetBytes(expectedData), publicKey);
            Assert.ThrowsException<ArgumentException>(() => dataEncryptor.AsymmetricDecryptData(encrypedData, differentPrivateKey));

            encrypedData = dataEncryptor.AsymmetricEncryptData(Encoding.Default.GetBytes(expectedData), publicKey);
            Assert.ThrowsException<ArgumentException>(() => dataEncryptor.AsymmetricDecryptData(encrypedData, publicKey));

        }
    }
}
