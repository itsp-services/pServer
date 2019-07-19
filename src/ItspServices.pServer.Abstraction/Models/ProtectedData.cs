using System.Collections.Generic;

namespace ItspServices.pServer.Abstraction.Models
{
    public class ProtectedData
    {
        public int Id { get; set; }
        public int FolderId { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public List<byte[]> KeyRegister { get; set; }
    }
}
