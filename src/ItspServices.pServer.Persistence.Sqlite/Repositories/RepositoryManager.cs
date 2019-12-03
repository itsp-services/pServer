using System.Data.Common;
using System.IO;
using System.Reflection;
using ItspServices.pServer.Abstraction.Repository;
using Microsoft.Extensions.Options;

namespace ItspServices.pServer.Persistence.Sqlite.Repositories
{
    class RepositoryManager : IRepositoryManager
    {
        public IUserRepository UserRepository { get; }
        public IProtectedDataRepository ProtectedDataRepository => throw new System.NotImplementedException();

        public RepositoryManager(IOptions<PersistenceOption> options)
        {
            InitSchema(options.Value.ProviderFactory, options.Value.ConnectionString);
            UserRepository = new UserRepository(options.Value.ProviderFactory, options.Value.ConnectionString, options.Value.ServerRoles);
        }

        private void InitSchema(DbProviderFactory providerFactory, string connectionString)
        {
            string initSqlScript = ReadInitSql();
            using (DbConnection con = providerFactory.CreateConnection())
            {
                con.ConnectionString = connectionString;
                con.Open();

                using (DbCommand init = con.CreateCommand())
                {
                    init.CommandText = initSqlScript;
                    init.ExecuteNonQuery();
                }
            }
        }

        private string ReadInitSql()
        {
            string initSqlScript;
            Assembly assembly = typeof(RepositoryManager).GetTypeInfo().Assembly;
            Stream schemaSqlResource = assembly.GetManifestResourceStream("ItspServices.pServer.Persistence.Sqlite.DatabaseScripts.DBInit.sql");
            using (TextReader reader = new StreamReader(schemaSqlResource))
                initSqlScript = reader.ReadToEnd();
            return initSqlScript;
        }
    }
}
