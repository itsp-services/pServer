using System;
using System.IO;
using System.Security.Cryptography;

namespace ItspServices.pServer.Client.Security
{
    class DataEncryptor : IDataEncryptor
    {

        public DataEncryptor(){ }

        public string CreateSymmetricKey()
        {
            throw new NotImplementedException();
        }

        public string SymmetricEncryptData(string data, string key)
        {
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
            byte[] symmetricKey = Convert.FromBase64String(key.Substring(0, key.IndexOf(',')));
            byte[] symmetricIV = Convert.FromBase64String(key.Substring(key.IndexOf(',') + 1));
            ICryptoTransform encryptor = aesProvider.CreateEncryptor(symmetricKey, symmetricIV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string SymmetricDecryptData(string data, string key)
        {
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
            byte[] symmetricKey = Convert.FromBase64String(key.Substring(0, key.IndexOf(',')));
            byte[] symmetricIV = Convert.FromBase64String(key.Substring(key.IndexOf(',') + 1));
            ICryptoTransform decryptor = aesProvider.CreateDecryptor(symmetricKey, symmetricIV);
            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(data)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        public string AsymmetricEncryptData(string data, string key)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048);
            rsaProvider.ImportCspBlob(Convert.FromBase64String(key));
            return Convert.ToBase64String(rsaProvider.Encrypt(Convert.FromBase64String(data), false));
        }

        public string AsymmetricDecryptData(string data, string key)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048);
            rsaProvider.ImportCspBlob(Convert.FromBase64String(key));
            return Convert.ToBase64String(rsaProvider.Decrypt(Convert.FromBase64String(data), false));
        }
    }
}
