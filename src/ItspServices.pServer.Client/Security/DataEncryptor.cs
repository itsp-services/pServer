using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ItspServices.pServer.Client.Security
{
    class DataEncryptor : IDataEncryptor
    {
        public DataEncryptor() { }

        public byte[] CreateSymmetricKey(int keySize = 128)
        {
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();

            if (!aesProvider.ValidKeySize(keySize))
                throw new ArgumentException("Invalid key size.");

            aesProvider.KeySize = keySize;
            aesProvider.GenerateKey();
            aesProvider.GenerateIV();
            int keyByteLength = aesProvider.KeySize / 8;
            byte[] combinedKey = new byte[keyByteLength + aesProvider.IV.Length];
            aesProvider.Key.CopyTo(combinedKey, 0);
            aesProvider.IV.CopyTo(combinedKey, keyByteLength);
            return combinedKey;
        }

        public byte[] SymmetricEncryptData(byte[] data, byte[] key)
        {
            int keyByteLength = key.Length / 2;
            int keyLength = keyByteLength * 8;
            byte[] symmetricKey = new byte[keyByteLength];
            byte[] symmetricIV = new byte[keyByteLength];

            for (int i = 0; i < keyByteLength; i++)
            {
                symmetricKey[i] = key[i];
                symmetricIV[i] = key[i + keyByteLength];
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


        public byte[] SymmetricDecryptData(byte[] data, byte[] key)
        {
            int keyByteLength = key.Length / 2;
            int keyLength = keyByteLength * 8;
            byte[] symmetricKey = new byte[keyByteLength];
            byte[] symmetricIV = new byte[keyByteLength];

            for (int i = 0; i < keyByteLength; i++)
            {
                symmetricKey[i] = key[i];
                symmetricIV[i] = key[i + keyByteLength];
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

        public byte[] AsymmetricEncryptData(byte[] data, byte[] key)
        {
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048))
            {
                rsaProvider.ImportCspBlob(key);
                return rsaProvider.Encrypt(data, false);
            }
        }

        public byte[] AsymmetricDecryptData(byte[] data, byte[] key)
        {
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048))
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
