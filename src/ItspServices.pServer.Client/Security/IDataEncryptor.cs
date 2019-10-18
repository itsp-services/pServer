namespace ItspServices.pServer.Client.Security
{
    public interface IDataEncryptor
    {
        byte[] EncryptWithSymmetricKey(byte[] data, byte[] keydata);
    }
}
