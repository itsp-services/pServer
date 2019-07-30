using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction.Models
{
    public class SymmetricKey
    {
        public int MatchingPublicKeyId { get; set; }
        public byte[] KeyData { get; set; }
    }
}
