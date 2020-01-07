using ItspServices.pServer.Client.Models;

namespace ItspServices.pServer.Client.Security
{
    public interface ILocalKeysController
    {
        string GetPublicKey();
        string GetPrivateKey();
    }
}
