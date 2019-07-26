using System.Text;

namespace ItspServices.pServer.Abstraction.Models
{
    public class Key
    {
        public int Id { get; set; } = -1;
        public byte[] KeyData { get; set; }
        public KeyFlag Flag { get; set; } = KeyFlag.ACTIVE;

        public enum KeyFlag
        {
            ACTIVE,
            OBSOLET
        }
    }
}
