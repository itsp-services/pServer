using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItspServices.pServer.Models
{
    public class KeyPairModel
    {
        public byte[] PublicKey { get; set; }
        public byte[] SymmetricKey { get; set; }
    }
}
