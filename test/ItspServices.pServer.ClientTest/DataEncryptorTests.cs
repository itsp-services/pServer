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

        }


        [TestMethod]
        public void SymmetricEncryptAndDecryptData_ShouldReturnCorrectData()
        {
            string expectedData = Convert.ToBase64String(Encoding.Default.GetBytes("SecretPassword"));
            DataEncryptor dataEncryptor = new DataEncryptor();
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
            aesProvider.KeySize = 128;
            aesProvider.GenerateKey();
            aesProvider.GenerateIV();
            int keyByteLength = aesProvider.KeySize / 8;
            byte[] combinedKey = new byte[keyByteLength * 2];
            aesProvider.Key.CopyTo(combinedKey, 0);
            aesProvider.IV.CopyTo(combinedKey, keyByteLength);
            byte[] encrypedData = dataEncryptor.SymmetricEncryptData(Convert.FromBase64String(expectedData), combinedKey);
            string decrypedData = Encoding.Default.GetString(dataEncryptor.SymmetricDecryptData(encrypedData, combinedKey));
            Assert.AreEqual(Encoding.Default.GetString(Convert.FromBase64String(expectedData)), decrypedData);
        }

        [TestMethod]
        public void AsymmetricEncryptAndDecryptData_ShouldReturnCorrectData()
        {
            string expectedData = Convert.ToBase64String(Encoding.Default.GetBytes("SecretPassword"));
            DataEncryptor dataEncryptor = new DataEncryptor();
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            byte[] publicKey = rsaProvider.ExportCspBlob(false);
            byte[] privateKey = rsaProvider.ExportCspBlob(true);
            string encrypedData = Convert.ToBase64String(dataEncryptor.AsymmetricEncryptData(Convert.FromBase64String(expectedData), publicKey));
            string decrypedData = Convert.ToBase64String(dataEncryptor.AsymmetricDecryptData(Convert.FromBase64String(encrypedData), privateKey));
            Assert.AreEqual(expectedData, decrypedData);
        }
    }
}
