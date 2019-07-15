using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ItspServices.pServer.Abstraction.Models
{
    public class User : IdentityUser
    {
        public List<byte[]> PublicKeys { get; }

        public User()
        {
            PublicKeys = new List<byte[]>();
        }
    }
}
