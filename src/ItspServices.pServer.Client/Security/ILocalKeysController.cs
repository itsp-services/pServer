using ItspServices.pServer.Client.Models;

namespace ItspServices.pServer.Client.Security
{
    public interface ILocalKeysController
    {
        string CreateSymmetricKey();
        string GetPublicKey();
        string GetPrivateKey();
    }
}
