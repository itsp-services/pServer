using System;
using System.IO;

namespace ItspServices.pServer.Client.Security
{
    class LocalKeysController : ILocalKeysController
    {
        private string _defaultDestination = @"C:\temp\";

        public LocalKeysController() { }

        public void SavePublicKey(string destination, string key)
        {
            byte[] byteKey = Convert.FromBase64String(key);
            File.WriteAllBytes(destination, byteKey);
        }

        public string SavePublicKey(string key)
        {
            string destination = _defaultDestination + "publicKey.txt";
            SavePublicKey(destination, key);
            return destination;
        }

        public void SavePrivateKey(string destination, string key)
        {
            byte[] byteKey = Convert.FromBase64String(key);
            File.WriteAllBytes(destination, byteKey);
        }

        public string SavePrivateKey(string key)
        {
            string destination = _defaultDestination + "privateKey.txt";
            SavePrivateKey(destination, key);
            return destination;
        }

        public string GetPublicKey(string destination)
        {
            return Convert.ToBase64String(File.ReadAllBytes(destination));

        }

        public string GetPrivateKey(string destination)
        {
            return Convert.ToBase64String(File.ReadAllBytes(destination));
        }
    }
}