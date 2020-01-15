namespace ItspServices.pServer.Client.Security
{
    public interface ILocalKeysController
    {
        void SavePublicKey(string destination, string key);
        string SavePublicKey(string key);
        void SavePrivateKey(string destination, string key);
        string SavePrivateKey(string key);
        string GetPublicKey(string destination);
        string GetPrivateKey(string destination);
    }
}
