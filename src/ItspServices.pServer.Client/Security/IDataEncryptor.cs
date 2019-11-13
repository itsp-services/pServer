using ItspServices.pServer.Client.Models;

namespace ItspServices.pServer.Client.Security
{
    public interface IDataEncryptor
    {
        string EncryptData(string data, string key);
        string DecryptData(string data, string key);
    }
}
