using System.Data.Common;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ItspServices.pServer.ServerTest")]
namespace ItspServices.pServer.Persistence.Sqlite.Repositories
{
    class SqlUserRepository
    {
        private DbProviderFactory _sqlFactory;
        private readonly string _connectionString;

        public SqlUserRepository(DbProviderFactory sqlFactory, SqlOptions options)
        {
            _sqlFactory = sqlFactory;
            _connectionString = options.ConnectionString;
        }
    }
}
