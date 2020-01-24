using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using ItspServices.pServer.Client.Datatypes;

namespace ItspServices.pServer.Client.Security
{
    class DataEncryptor : IDataEncryptor
    {
        private readonly int _aesIdentifikationVectorLength = 128;

        public byte[] SymmetricEncryptData(byte[] data, Key key)
        {
            int keyByteLength = key.GetBytes().Length - _aesIdentifikationVectorLength / 8;
            int keyLength = keyByteLength * 8;
            Key symmetricKey = new byte[keyByteLength];
            Key symmetricIV = new byte[_aesIdentifikationVectorLength / 8];

            for (int i = 0; i < keyByteLength; i++)
            {
                symmetricKey.GetBytes()[i] = key.GetBytes()[i];
            }
            for (int i = 0; i < _aesIdentifikationVectorLength / 8; i++)
            {
                symmetricIV.GetBytes()[i] = key.GetBytes()[i + keyByteLength];
            }

            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.KeySize = keyLength;
                aesProvider.Key = symmetricKey;
                aesProvider.IV = symmetricIV;
                ICryptoTransform encryptor = aesProvider.CreateEncryptor(aesProvider.Key, aesProvider.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(Encoding.Default.GetString(data));
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        public byte[] SymmetricDecryptData(byte[] data, Key key)
        {
            int keyByteLength = key.GetBytes().Length - _aesIdentifikationVectorLength / 8;
            int keyLength = keyByteLength * 8;
            Key symmetricKey = new byte[keyByteLength];
            Key symmetricIV = new byte[_aesIdentifikationVectorLength / 8];

            for (int i = 0; i < keyByteLength; i++)
            {
                symmetricKey.GetBytes()[i] = key.GetBytes()[i];
            }
            for (int i = 0; i < _aesIdentifikationVectorLength / 8; i++)
            {
                symmetricIV.GetBytes()[i] = key.GetBytes()[i + keyByteLength];
            }

            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.KeySize = keyLength;
                aesProvider.Key = symmetricKey;
                aesProvider.IV = symmetricIV;
                ICryptoTransform decryptor = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);

                try
                {
                    using (MemoryStream msDecrypt = new MemoryStream(data))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return Encoding.Default.GetBytes(srDecrypt.ReadToEnd());
                            }
                        }
                    }
                }
                catch (CryptographicException)
                {
                    throw new ArgumentException("Wrong symmetric key.");
                }
            }
        }

        public byte[] AsymmetricEncryptData(byte[] data, Key key)
        {
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider())
            {
                rsaProvider.ImportCspBlob(key);
                return rsaProvider.Encrypt(data, false);
            }
        }

        public byte[] AsymmetricDecryptData(byte[] data, Key key)
        {
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider())
            {
                rsaProvider.ImportCspBlob(key);
                try
                {
                    return rsaProvider.Decrypt(data, false);
                }
                catch (CryptographicException)
                {
                    throw new ArgumentException("Wrong key.");
                }
            }
        }
    }
}
