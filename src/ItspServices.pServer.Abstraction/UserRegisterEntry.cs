using ItspServices.pServer.Abstraction.Models;
using System.Collections.Generic;

namespace ItspServices.pServer.Abstraction
{
    public class UserRegisterEntry
    {
        public User User { get; set; }
        public List<SymmetricKey> EncryptedKeys { get; set; } = new List<SymmetricKey>();
        public Permission Permission { get; set; } = Permission.VIEW;
    }

    public enum Permission
    {
        VIEW = 0, READ = 1, WRITE = 2
    }
}