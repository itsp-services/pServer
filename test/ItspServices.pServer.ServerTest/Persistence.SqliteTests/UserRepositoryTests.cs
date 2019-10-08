using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Persistence.Sqlite.Repositories;
using System.Reflection;
using System.IO;

namespace ItspServices.pServer.ServerTest.Persistence.SqliteTests
{
    [TestClass]
    public class UserRepositoryTests
    {
        private TestContext _testContext;
        public TestContext TestContext { 
            get { return _testContext; } 
            set { _testContext = value; } 
        }

        DbConnection memoryDbConnection;
        UserRepository repository;

        #region Initialize and cleanup methods

        [TestInitialize]
        public void Init()
        {
            string testConnectionString = $"Data Source={_testContext.TestName}; Mode=Memory; Cache=Shared";
            memoryDbConnection = SqliteFactory.Instance.CreateConnection();
            memoryDbConnection.ConnectionString = testConnectionString;
            // A connection has to be open during the tests so the in-memory
            // database will not be freed each time the repository closes the connection.
            memoryDbConnection.Open();
            InitSchema(memoryDbConnection);
            repository = new UserRepository(SqliteFactory.Instance, testConnectionString);
        }

        private static void InitSchema(DbConnection con)
        {
            Assembly assembly = typeof(UserRepository).GetTypeInfo().Assembly;
            Stream schemaSqlResource = assembly.GetManifestResourceStream("ItspServices.pServer.Persistence.Sqlite.DatabaseScripts.DBInit.sql");
            DbCommand initSchema = SqliteFactory.Instance.CreateCommand();
            initSchema.Connection = con;
            using (TextReader reader = new StreamReader(schemaSqlResource))
                initSchema.CommandText = reader.ReadToEnd();
            initSchema.ExecuteNonQuery();
        }

        [TestCleanup]
        public void Cleanup()
        {
            repository = null;
            memoryDbConnection.Close();
        }

        #endregion

        [TestMethod]
        public void GetUserById_ShouldSucceed()
        {

        }
    }
}
