using ItspServices.pServer.Client.Datatypes;

namespace ItspServices.pServer.Client.Security.Keys
{
    interface IKeyFactory
    {
        Key CreateSymmetricKey(int keysize = 128);
        AsymmetricKeyPair CreateAsymmetricKeyPair(int keysize = 2048);
    }
}
