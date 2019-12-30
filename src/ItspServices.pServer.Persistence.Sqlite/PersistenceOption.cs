using System.Collections.Generic;
using System.Data.Common;

namespace ItspServices.pServer.Persistence.Sqlite
{
    public class PersistenceOption
    {
        public string ConnectionString { get; set; }
        public DbProviderFactory ProviderFactory { get; set; }
        public ICollection<string> ServerRoles { get; set; }
    }
}
