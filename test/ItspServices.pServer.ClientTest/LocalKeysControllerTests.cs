using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Client.Datatypes;
using ItspServices.pServer.Client.Security;

namespace ItspServices.pServer.ClientTest
{
    [TestClass]
    public class LocalKeysControllerTests
    {
        [TestMethod]
        public void SaveKey_ShouldReturnPath()
        {
            Key publicKey = Encoding.Default.GetBytes("publicKey");
            Key privateKey = Encoding.Default.GetBytes("privateKey");
            LocalKeysController keysController = new LocalKeysController();

            string publicKeyPath = keysController.SavePublicKey(publicKey.GetBase64());
            Assert.IsNotNull(publicKeyPath);
            Assert.IsTrue(publicKeyPath.Contains(@"\") && publicKeyPath.Contains(@".txt"));

            string privateKeyPath = keysController.SavePrivateKey(privateKey.GetBase64());
            Assert.IsNotNull(privateKeyPath);
            Assert.IsTrue(privateKeyPath.Contains(@"\") && privateKeyPath.Contains(@".txt"));
        }

        [TestMethod]
        public void GetKey_ShouldThrowException()
        {
            Key key = null;
            LocalKeysController keysController = new LocalKeysController();

            Assert.ThrowsException<FileNotFoundException>(() => key = Encoding.Default.GetBytes(keysController.GetPublicKey(@"C:\temp\wrongPublicKey.txt")));

            Assert.ThrowsException<FileNotFoundException>(() => key = Encoding.Default.GetBytes(keysController.GetPrivateKey(@"C:\temp\wrongPrivateKey.txt")));
        }

        [TestMethod]
        public void SaveAndGetKey_ShouldReturnOriginalKey()
        {
            Key publicKey = Encoding.Default.GetBytes("publicKey");
            Key privateKey = Encoding.Default.GetBytes("privateKey");
            string path;
            LocalKeysController keysController = new LocalKeysController();

            path = keysController.SavePublicKey(publicKey.GetBase64());
            Key returnedPublicKey = new Key(keysController.GetPublicKey(path));
            Assert.AreEqual(publicKey.GetBase64(), returnedPublicKey.GetBase64());

            path = keysController.SavePrivateKey(privateKey.GetBase64());
            Key returnedPrivateKey = new Key(keysController.GetPrivateKey(path));
            Assert.AreEqual(privateKey.GetBase64(), returnedPrivateKey.GetBase64());
        }
    }
}
