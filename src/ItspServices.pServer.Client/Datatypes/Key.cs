using System;

namespace ItspServices.pServer.Client.Datatypes
{
    internal class Key
    {
        private byte[] _data;

        public Key(string base64Key)
            => _data = Convert.FromBase64String(base64Key);

        public Key(byte[] byteKey)
            => _data = byteKey;

        public string GetBase64()
            => Convert.ToBase64String(_data);

        public void SetWithBase64(string base64Key)
            => _data = Convert.FromBase64String(base64Key);

        public byte[] GetBytes()
            => _data;

        public void SetWithBytes(byte[] byteKey)
            => _data = byteKey;

        public static implicit operator byte[](Key key)
            => key.GetBytes();

        public static implicit operator Key(byte[] byteKey)
            => new Key(byteKey);
    }
}
