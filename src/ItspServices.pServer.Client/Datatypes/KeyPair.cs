using ItspServices.pServer.Client.Models;

namespace ItspServices.pServer.Client.Datatypes
{
    internal class KeyPair
    {
        public Key PublicKey { get; set; }
        public Key SymmetricKey { get; set; }

        public KeyPair() { }

        public KeyPair(KeyPairModel keyPairModel)
        {
            PublicKey = new Key(keyPairModel.PublicKey);
            SymmetricKey = new Key(keyPairModel.SymmetricKey);
        }

        public void InitializeWithKeyPairModel(KeyPairModel keyPairModel)
        {
            this.PublicKey = new Key(keyPairModel.PublicKey);
            this.SymmetricKey = new Key(keyPairModel.SymmetricKey);
        }

        public KeyPairModel ToKeyPairModel()
        {
            return new KeyPairModel
            {
                PublicKey = this.PublicKey.GetBase64(),
                SymmetricKey = this.SymmetricKey.GetBase64()
            };
        }
    }
}
