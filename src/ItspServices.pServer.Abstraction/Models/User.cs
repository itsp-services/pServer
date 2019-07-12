using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction.Models
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public List<byte[]> PublicKeys { get; }

        public User()
        {
            PublicKeys = new List<byte[]>();
        }
    }
}
