using System.Collections.Generic;

namespace ItspServices.pServer.Client.Models
{
    class DataModel
    {
        public string Name { get; set; }
        public string Data { get; set; }
        public IEnumerable<KeyPairModel> KeyPairs { get; set; }
    }
}