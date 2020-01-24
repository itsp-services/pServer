using System;
using System.Text;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Client.Datatypes;
using ItspServices.pServer.Client.Security;
using ItspServices.pServer.Client.Security.Keys;

namespace ItspServices.pServer.ClientTest
{
    [TestClass]
    public class DataEncryptorTests
    {
        private readonly IKeyFactory keyFactory = new DefaultKeyFactory();

        [TestMethod]
        public void CreateSymmetricKey_ShouldReturnValidKey()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();
            Key symmetricKey;
            int aesIdentifikationVectorLength = 128;

            symmetricKey = keyFactory.CreateSymmetricKey();
            Assert.IsNotNull(symmetricKey);
            Assert.AreEqual(aesIdentifikationVectorLength / 8 + 128 / 8, symmetricKey.GetBytes().Length);

            symmetricKey = keyFactory.CreateSymmetricKey(256);
            Assert.IsNotNull(symmetricKey);
            Assert.AreEqual(aesIdentifikationVectorLength / 8 + 256 / 8, symmetricKey.GetBytes().Length);

            Assert.ThrowsException<ArgumentException>(() => symmetricKey = keyFactory.CreateSymmetricKey(333));
        }

        [TestMethod]
        public void CreateTwoSymmetricKeys_ShouldReturnDifferentKeys()
        {
            Key symmetricKey = keyFactory.CreateSymmetricKey();
            Key differentSymmetricKey = keyFactory.CreateSymmetricKey();

            Assert.AreNotEqual(symmetricKey.GetBytes(), differentSymmetricKey.GetBytes());
            Assert.AreNotEqual(symmetricKey, differentSymmetricKey);
        }

        [TestMethod]
        public void SymmetricEncryptAndDecryptData_ShouldReturnCorrectData()
        {
            string expectedData = "SecretPassword";
            DataEncryptor dataEncryptor = new DataEncryptor();
            Key key = keyFactory.CreateSymmetricKey();

            byte[] encrypedData = dataEncryptor.SymmetricEncryptData(Encoding.Default.GetBytes(expectedData), key);
            byte[] decrypedData = dataEncryptor.SymmetricDecryptData(encrypedData, key);

            Assert.AreEqual(expectedData, Encoding.Default.GetString(decrypedData));
        }

        [TestMethod]
        public void SymmetricEncryptAndDecryptDataWithDifferentKeySize_ShouldReturnCorrectData()
        {
            string expectedData = "SecretPassword";
            DataEncryptor dataEncryptor = new DataEncryptor();
            Key key = keyFactory.CreateSymmetricKey(256);

            byte[] encrypedData = dataEncryptor.SymmetricEncryptData(Encoding.Default.GetBytes(expectedData), key);
            byte[] decrypedData = dataEncryptor.SymmetricDecryptData(encrypedData, key);

            Assert.AreEqual(expectedData, Encoding.Default.GetString(decrypedData));
        }

        [TestMethod]
        public void SymmetricEncryptAndDecryptDataWithWrongKey_ShouldThrowException()
        {
            string expectedData = "SecretPassword";
            DataEncryptor dataEncryptor = new DataEncryptor();
            Key key = keyFactory.CreateSymmetricKey();

            byte[] encrypedData = dataEncryptor.SymmetricEncryptData(Encoding.Default.GetBytes(expectedData), key);
            byte[] differentKey = keyFactory.CreateSymmetricKey();

            Assert.ThrowsException<ArgumentException>(() => dataEncryptor.SymmetricDecryptData(encrypedData, differentKey));
        }

        [TestMethod]
        public void CreateAsymmetricKeyPair_ShouldReturnValidKeys()
        {
            DataEncryptor dataEncryptor = new DataEncryptor();

            var asymmetricKeyPair = keyFactory.CreateAsymmetricKeyPair();
            Assert.IsNotNull(asymmetricKeyPair);

            asymmetricKeyPair = keyFactory.CreateAsymmetricKeyPair(1024);
            Assert.IsNotNull(asymmetricKeyPair);
            Assert.AreNotEqual(asymmetricKeyPair.PrivateKey.GetBytes().Length, keyFactory.CreateAsymmetricKeyPair(2048).PrivateKey.GetBytes().Length);

            Assert.ThrowsException<ArgumentException>(() => asymmetricKeyPair = keyFactory.CreateAsymmetricKeyPair(256));

            Assert.ThrowsException<ArgumentException>(() => asymmetricKeyPair = keyFactory.CreateAsymmetricKeyPair(32768));

            Assert.ThrowsException<ArgumentException>(() => asymmetricKeyPair = keyFactory.CreateAsymmetricKeyPair(3333));
        }

        [TestMethod]
        public void AsymmetricEncryptAndDecryptData_ShouldReturnCorrectData()
        {
            string expectedData = "SecretPassword";
            DataEncryptor dataEncryptor = new DataEncryptor();
            var asymmetricKeyPair = keyFactory.CreateAsymmetricKeyPair();

            byte[] encrypedData = dataEncryptor.AsymmetricEncryptData(Encoding.Default.GetBytes(expectedData), asymmetricKeyPair.PublicKey);
            byte[] decrypedData = dataEncryptor.AsymmetricDecryptData(encrypedData, asymmetricKeyPair.PrivateKey);

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
            encrypedData = dataEncryptor.AsymmetricEncryptData(Encoding.Default.GetBytes(expectedData), publicKey);
            byte[] differentPrivateKey = differentRsaProvider.ExportCspBlob(true);
            Assert.ThrowsException<ArgumentException>(() => dataEncryptor.AsymmetricDecryptData(encrypedData, differentPrivateKey));

            encrypedData = dataEncryptor.AsymmetricEncryptData(Encoding.Default.GetBytes(expectedData), publicKey);
            Assert.ThrowsException<ArgumentException>(() => dataEncryptor.AsymmetricDecryptData(encrypedData, publicKey));
        }
    }
}
