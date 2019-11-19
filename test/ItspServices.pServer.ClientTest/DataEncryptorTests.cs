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
            aesProvider.GenerateKey();
            aesProvider.GenerateIV();
            string encrypedData = dataEncryptor.SymmetricEncryptData(expectedData, $"{Convert.ToBase64String(aesProvider.Key)},{Convert.ToBase64String(aesProvider.IV)}");
            string decrypedData = dataEncryptor.SymmetricDecryptData(encrypedData, $"{Convert.ToBase64String(aesProvider.Key)},{Convert.ToBase64String(aesProvider.IV)}");
            Assert.AreEqual(expectedData, decrypedData);
        }

        [TestMethod]
        public void AsymmetricEncryptAndDecryptData_ShouldReturnCorrectData()
        {
            string expectedData = Convert.ToBase64String(Encoding.Default.GetBytes("SecretPassword"));
            DataEncryptor dataEncryptor = new DataEncryptor();
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            string publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
            string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));
            string encrypedData = dataEncryptor.AsymmetricEncryptData(expectedData, publicKey);
            string decrypedData = dataEncryptor.AsymmetricDecryptData(encrypedData, privateKey);
            Assert.AreEqual(expectedData, decrypedData);
        }
    }
}
