using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Persistence.Sqlite.Repositories;
using System.Reflection;
using System.IO;
using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.ServerTest.Persistence.SqliteTests
{
    [TestClass]
    public class UserRepositoryTests
    {
        static string InitSQLScript;

        private TestContext _testContext;
        public TestContext TestContext { 
            get { return _testContext; } 
            set { _testContext = value; } 
        }

        DbConnection memoryDbConnection;
        UserRepository repository;

        #region Initialize and cleanup methods

        [ClassInitialize]
        public static void ClassInit(TestContext _)
        {
            Assembly assembly = typeof(UserRepository).GetTypeInfo().Assembly;
            Stream schemaSqlResource = assembly.GetManifestResourceStream("ItspServices.pServer.Persistence.Sqlite.DatabaseScripts.DBInit.sql");
            using (TextReader reader = new StreamReader(schemaSqlResource))
                InitSQLScript = reader.ReadToEnd();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            InitSQLScript = null;
        }

        [TestInitialize]
        public void Init()
        {
            string testConnectionString = $"Data Source={_testContext.TestName}; Mode=Memory; Cache=Shared";
            memoryDbConnection = SqliteFactory.Instance.CreateConnection();
            memoryDbConnection.ConnectionString = testConnectionString;
            // A connection has to be open during the tests so the in-memory
            // database will not be freed each time the repository closes the connection.
            memoryDbConnection.Open();

            DbCommand init = memoryDbConnection.CreateCommand();
            init.CommandText = InitSQLScript;
            init.ExecuteNonQuery();

            repository = new UserRepository(SqliteFactory.Instance, testConnectionString);
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
            DbCommand insertTestData = memoryDbConnection.CreateCommand();
            insertTestData.CommandText = "INSERT INTO Roles ('Name') VALUES ('User');"
                                       + "INSERT INTO Users ('Username', 'PasswordHash', 'RoleID') VALUES "
                                       + "('FooUser', 'SecretPasswordHash', 1)";
            insertTestData.ExecuteNonQuery();

            User fooUser = repository.GetById(1);

            Assert.AreEqual(1, fooUser.Id);
            Assert.AreEqual("FooUser", fooUser.UserName);
            Assert.AreEqual("FooUser".Normalize(), fooUser.NormalizedUserName);
            Assert.AreEqual("User", fooUser.Role);
        }
    }
}
