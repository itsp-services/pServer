using System;

namespace ItspServices.pServer.Client.Security
{
    class DataEncryptor : IDataEncryptor
    {

        public DataEncryptor(){ }

        public string SymmetricEncryptData(string data, string key)
        {
            throw new NotImplementedException();
        }
        public string SymmetricDecryptData(string data, string key)
        {
            throw new NotImplementedException();
        }

        public string AsymmetricEncryptData(string data, string key)
        {
            throw new NotImplementedException();
        }
        public string AsymmetricDecryptData(string data, string key)
        {
            throw new NotImplementedException();
        }
    }
}
