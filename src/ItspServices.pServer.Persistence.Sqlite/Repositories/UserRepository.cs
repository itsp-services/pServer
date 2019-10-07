using System.Data.Common;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ItspServices.pServer.ServerTest")]
namespace ItspServices.pServer.Persistence.Sqlite.Repositories
{
    class UserRepository
    {
        private DbProviderFactory _sqlFactory;
        private readonly string _connectionString;

        public UserRepository(DbProviderFactory sqlFactory, string connectionString)
        {
            _sqlFactory = sqlFactory;
            _connectionString = connectionString;
        }
    }
}
