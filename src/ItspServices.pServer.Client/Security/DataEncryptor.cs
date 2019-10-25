using System;
using System.Text;

namespace ItspServices.pServer.Client.Security
{
    class DataEncryptor : IDataEncryptor
    {

        public DataEncryptor(){}

        public byte[] EncryptWithSymmetricKey(byte[] data, byte[] keydata)
        {
            throw new NotImplementedException();
        }
        public byte[] DecryptWithSymmetricKey(byte[] data, byte[] keydata)
        {
            throw new NotImplementedException();
        }
    }
}
