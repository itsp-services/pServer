using System.Text;

namespace ItspServices.pServer.Abstraction.Models
{
    public class Key
    {
        private byte[] _key;

        public Key()
        {
        }

        public Key(byte[] key)
        {
            _key = (byte[]) key.Clone();
        }

        public Key(string key)
        {
            _key = Encoding.UTF8.GetBytes(key);
        }
        
        public string GetKeyAsString()
        {
            return Encoding.UTF8.GetString(_key);
        }

        public byte[] GetKeyAsBytes()
        {
            return (byte[]) _key.Clone();
        }

        public void setKey(string key)
        {
            _key = Encoding.UTF8.GetBytes(key);
        }

        public void setKey(byte[] key)
        {
            _key = (byte[]) key.Clone();
        }
    }
}
