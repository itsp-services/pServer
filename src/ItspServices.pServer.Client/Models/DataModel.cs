using System.Collections.Generic;

namespace ItspServices.pServer.Client.Models
{
    class DataModel
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public IEnumerable<KeyPairModel> KeyPairs { get; set; }
    }
}