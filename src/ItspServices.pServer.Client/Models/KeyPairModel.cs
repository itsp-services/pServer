using ItspServices.pServer.Client.Datatypes;

namespace ItspServices.pServer.Client.Models
{
    class KeyPairModel
    {
        public string PublicKey { get; set; }
        public string SymmetricKey { get; set; }
    }

    static class KeyPairModelExtensions
    {
        public static KeyPairModel ToKeyPairModel(this KeyPair keyPair)
        {
            return new KeyPairModel()
            {
                PublicKey = keyPair.PublicKey.GetBase64(),
                SymmetricKey = keyPair.SymmetricKey.GetBase64()
            };
        }

        public static KeyPair ToKeyPair(this KeyPairModel keyPairModel)
        {
            KeyPair keyPair = new KeyPair()
            {
                PublicKey = new Key(keyPairModel.PublicKey),
                SymmetricKey = new Key(keyPairModel.SymmetricKey)
            };
            return keyPair;
        }
    }
}
