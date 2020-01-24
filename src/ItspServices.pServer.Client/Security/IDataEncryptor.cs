using ItspServices.pServer.Client.Datatypes;

namespace ItspServices.pServer.Client.Security
{
    interface IDataEncryptor
    {
        byte[] SymmetricEncryptData(byte[] data, Key key);
        byte[] SymmetricDecryptData(byte[] data, Key key);
        byte[] AsymmetricEncryptData(byte[] data, Key key);
        byte[] AsymmetricDecryptData(byte[] data, Key key);
    }
}
