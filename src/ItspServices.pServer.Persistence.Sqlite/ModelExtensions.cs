using System;
using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Persistence.Sqlite
{
    internal static class ModelExtensions
    {
        public static string AsBase64String(this Key k)
        {
            return Convert.ToBase64String(k.KeyData);
        }

        public static bool HasKeys(this User u)
        {
            return u.PublicKeys.Count > 0;
        }
    }
}
