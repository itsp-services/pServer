using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ItspServices.pServer.Abstraction.Models
{
    public class User
    {
        public UInt64 Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }

        public List<byte[]> PublicKeys { get; }

        public User()
        {
            PublicKeys = new List<byte[]>();
        }
    }
}
