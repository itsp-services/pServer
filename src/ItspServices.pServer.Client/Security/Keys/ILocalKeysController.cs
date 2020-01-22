namespace ItspServices.pServer.Client.Security.Keys
{
    public interface ILocalKeysController
    {
        string SavePublicKey(string key);
        string SavePrivateKey(string key);
        string GetPublicKey();
        string GetPrivateKey();
    }
}
