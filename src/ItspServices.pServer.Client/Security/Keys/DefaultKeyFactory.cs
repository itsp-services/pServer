using System;
using System.Security.Cryptography;
using ItspServices.pServer.Client.Datatypes;

namespace ItspServices.pServer.Client.Security.Keys
{
    class DefaultKeyFactory : IKeyFactory
    {
        public AsymmetricKeyPair CreateAsymmetricKeyPair(int keysize = 2048)
        {
            RSACryptoServiceProvider rsaProvider;
            try
            {
                using (rsaProvider = new RSACryptoServiceProvider(keysize))
                    return new AsymmetricKeyPair()
                    {
                        PublicKey = new Key(rsaProvider.ExportCspBlob(false)),
                        PrivateKey = new Key(rsaProvider.ExportCspBlob(true))
                    };
            }
            catch (CryptographicException e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public Key CreateSymmetricKey(int keysize = 128)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                if (!aesProvider.ValidKeySize(keysize))
                    throw new ArgumentException("Invalid key size.");

                aesProvider.KeySize = keysize;
                aesProvider.GenerateKey();
                aesProvider.GenerateIV();
                int keyByteLength = aesProvider.KeySize / 8;
                Key combinedKey = new byte[keyByteLength + aesProvider.IV.Length];
                aesProvider.Key.CopyTo(combinedKey, 0);
                aesProvider.IV.CopyTo(combinedKey, keyByteLength);
                return combinedKey;
            }
        }
    }
}
