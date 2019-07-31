using System;
using System.Collections;
using System.Collections.Generic;

namespace ItspServices.pServer.Models
{
    public class DataModel
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public IEnumerable<KeyPairModel> KeyPairs { get; set; }
    }
}
