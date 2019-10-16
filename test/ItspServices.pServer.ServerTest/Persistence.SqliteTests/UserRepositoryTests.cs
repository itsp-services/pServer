using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Persistence.Sqlite.Repositories;
using System.Reflection;
using System.IO;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;
using System.Collections.Generic;
using System.Text;
using System.Data;

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

            using (DbCommand init = memoryDbConnection.CreateCommand())
            {
                init.CommandText = InitSQLScript;
                init.ExecuteNonQuery();
            }

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
            using (DbCommand insertTestData = memoryDbConnection.CreateCommand())
            {
                insertTestData.CommandText = "INSERT INTO Roles ('Name') VALUES ('User'), ('Admin');" +
                                             "INSERT INTO Users ('Username', 'PasswordHash', 'RoleID') VALUES " +
                                             "('FooUser', 'SecretPassword', 1)," +
                                             "('BarUser', 'OtherPassword', 2);" +
                                             "INSERT INTO PublicKeys ('UserID', 'PublicKeyNumber', 'KeyData', 'Active') VALUES " +
                                             "(1, 1, 'data', 1);";
                insertTestData.ExecuteNonQuery();
            }

            User fooUser = repository.GetById(1);

            Assert.AreEqual(1, fooUser.Id);
            Assert.AreEqual("FooUser", fooUser.UserName);
            Assert.AreEqual("FooUser".Normalize(), fooUser.NormalizedUserName);
            Assert.AreEqual("SecretPassword", fooUser.PasswordHash);
            Assert.AreEqual("User", fooUser.Role);
            Assert.AreEqual(1, fooUser.PublicKeys.Count);
            Assert.AreEqual(1, fooUser.PublicKeys[0].Id);
            Assert.AreEqual(Key.KeyFlag.ACTIVE, fooUser.PublicKeys[0].Flag);
            byte[] expectedKeyData = new byte[256];
            expectedKeyData[0] = (byte) 'd';
            expectedKeyData[1] = (byte) 'a';
            expectedKeyData[2] = (byte) 't';
            expectedKeyData[3] = (byte) 'a';
            CollectionAssert.AreEquivalent(expectedKeyData, fooUser.PublicKeys[0].KeyData);

            User barUser = repository.GetById(2);

            Assert.AreEqual(2, barUser.Id);
            Assert.AreEqual("BarUser", barUser.UserName);
            Assert.AreEqual("BarUser".Normalize(), barUser.NormalizedUserName);
            Assert.AreEqual("OtherPassword", barUser.PasswordHash);
            Assert.AreEqual("Admin", barUser.Role);
            Assert.AreEqual(0, barUser.PublicKeys.Count);
        }

        [TestMethod]
        public void GetUserByInvalidId_ShouldReturnNull()
        {
            User invalidUser = repository.GetById(999);
            Assert.IsNull(invalidUser);
        }

        [TestMethod]
        public void AddUser_ShouldSucceed()
        {
            using (DbCommand insertTestData = memoryDbConnection.CreateCommand())
            {
                insertTestData.CommandText = "INSERT INTO Roles ('Name') VALUES ('User');";
                insertTestData.ExecuteNonQuery();
            }

            using (IAddUnitOfWork<User> uow = repository.Add())
            {
                uow.Entity.UserName = "FooUser";
                uow.Entity.NormalizedUserName = uow.Entity.UserName.Normalize();
                uow.Entity.PasswordHash = "pw";
                uow.Entity.Role = "User";
                uow.Entity.PublicKeys = new List<Key>();
                uow.Entity.PublicKeys.Add(new Key() { 
                    KeyData = Encoding.UTF8.GetBytes("keydata"),
                    Flag = Key.KeyFlag.ACTIVE
                });

                uow.Complete();
            }

            using (DbCommand queryData = memoryDbConnection.CreateCommand())
            {
                queryData.CommandText = "SELECT * FROM Users;";
                using (IDataReader reader = queryData.ExecuteReader())
                {
                    Assert.IsTrue(reader.Read());
                    Assert.AreEqual(1, reader.GetInt32(0));
                    Assert.AreEqual("FooUser", reader.GetString(1));
                    Assert.AreEqual("pw", reader.GetString(2));
                    Assert.AreEqual(1, reader.GetInt32(3));
                    Assert.IsFalse(reader.Read());
                }
                queryData.CommandText = "SELECT * FROM PublicKeys;";
                using (IDataReader reader = queryData.ExecuteReader())
                {
                    reader.Read();
                }
            }
        }
    }
}
