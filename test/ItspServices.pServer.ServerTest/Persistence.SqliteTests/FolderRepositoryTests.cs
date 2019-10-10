using System.Data.Common;
using System.IO;
using System.Reflection;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Persistence.Sqlite.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ItspServices.pServer.ServerTest.Persistence.SqliteTests
{
    [TestClass]
    public class FolderRepositoryTests
    {
        static string InitSQLScript;
        private TestContext _testContext;
        public TestContext TestContext
        {
            get { return _testContext; }
            set { _testContext = value; }
        }

        DbConnection memoryDbConnection;
        FolderRepository repository;

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

            using (DbCommand init = memoryDbConnection.CreateCommand())
            {
                init.CommandText = InitSQLScript;
                init.ExecuteNonQuery();
            }

            repository = new FolderRepository(SqliteFactory.Instance, testConnectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            repository = null;
            memoryDbConnection.Close();
        }

        #endregion

        [TestMethod]
        public void GetRootFolderTest_ShouldSucced()
        {
            Folder root = repository.GetById(0);

            Assert.AreEqual("root", root.Name);
            Assert.AreEqual(0, root.Id);
            Assert.AreEqual(0, root.ParentId);
            Assert.IsTrue(root.SubfolderIds.Count == 0);
            Assert.IsTrue(root.DataIds.Count == 0);
        }

        [TestMethod]
        public void GetFolderById_ShouldSucced()
        {
            using (DbCommand command = memoryDbConnection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Folders('FolderName', 'Parent') VALUES " +
                                      "('testFolder', 0)";
                command.ExecuteNonQuery();
            }


        }
    }
}
