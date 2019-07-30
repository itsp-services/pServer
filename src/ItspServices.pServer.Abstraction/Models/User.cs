using System.Collections.Generic;


namespace ItspServices.pServer.Abstraction.Models
{
    public class User
    {
        public const int Invalid_Id = -1;
        public int Id { get; set; } = Invalid_Id;
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "User";
        public List<Key> PublicKeys { get; set; } = new List<Key>();
    }
}
