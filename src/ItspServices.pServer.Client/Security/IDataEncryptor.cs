using ItspServices.pServer.Client.Models;

namespace ItspServices.pServer.Client.Security
{
    public interface IDataEncryptor
    {
        string SymmetricEncryptData(string data, string key);
        string SymmetricDecryptData(string data, string key);
        string AsymmetricEncryptData(string data, string key);
        string AsymmetricDecryptData(string data, string key);
    }
}
