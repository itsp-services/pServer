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
        public List<byte[]> PublicKeys { get; set; } = new List<byte[]>();
    }
}
