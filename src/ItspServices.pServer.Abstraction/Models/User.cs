using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction.Models
{
    public class User
    {
        public User()
        {
            PublicKeys = new List<byte[]>();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public List<byte[]> PublicKeys { get; }
    }
}
