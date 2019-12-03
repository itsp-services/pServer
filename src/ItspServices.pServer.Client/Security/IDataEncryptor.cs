using ItspServices.pServer.Client.Models;

namespace ItspServices.pServer.Client.Security
{
    interface IDataEncryptor
    {
        byte[] CreateSymmetricKey();
        byte[] SymmetricEncryptData(byte[] data, byte[] key);
        byte[] SymmetricDecryptData(byte[] data, byte[] key);
        byte[] AsymmetricEncryptData(byte[] data, byte[] key);
        byte[] AsymmetricDecryptData(byte[] data, byte[] key);
    }
}
