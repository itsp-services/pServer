using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Persistence.Sqlite.Repositories;
using System.Reflection;
using System.IO;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;
using System.Text;
using System.Data;
using System;
using System.Collections.Generic;

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

            List<string> roles = new List<string>();
            roles.Add("User");
            roles.Add("Admin");
            repository = new UserRepository(SqliteFactory.Instance, testConnectionString, roles);
        }

        [TestCleanup]
        public void Cleanup()
        {
            repository = null;
            memoryDbConnection.Close();
        }

        #endregion

        [TestMethod]
        public void GetUserByNormalizedName_ShouldSucceed()
        {
            string keydata = Convert.ToBase64String(Encoding.Default.GetBytes("data"));
            using (DbCommand insertTestData = memoryDbConnection.CreateCommand())
            {
                insertTestData.CommandText = "INSERT INTO Users ('Username', 'PasswordHash', 'RoleID') VALUES " +
                                             "('Foo', 'SecretPassword', 1)," +
                                             "('Bar', 'OtherPassword', 2);" +
                                             "INSERT INTO PublicKeys ('UserID', 'PublicKeyNumber', 'KeyData', 'Active') VALUES " +
                                            $"(1, 1, '{keydata}', 1);";
                insertTestData.ExecuteNonQuery();
            }

            User u = repository.GetUserByNormalizedName("FOO");

            Assert.AreEqual(1, u.Id);
            Assert.AreEqual("Foo".ToUpper(), u.NormalizedUserName);
            Assert.AreEqual("Foo", u.UserName);
            Assert.AreEqual("SecretPassword", u.PasswordHash);
            Assert.AreEqual("User", u.Role);
            Assert.AreEqual(1, u.PublicKeys.Count);
            Assert.AreEqual(1, u.PublicKeys[0].Id);
            Assert.AreEqual(keydata, Convert.ToBase64String(u.PublicKeys[0].KeyData));
            Assert.AreEqual(Key.KeyFlag.ACTIVE, u.PublicKeys[0].Flag);

            u = repository.GetUserByNormalizedName("BAR");

            Assert.AreEqual(2, u.Id);
            Assert.AreEqual("Bar".ToUpper(), u.NormalizedUserName);
            Assert.AreEqual("Bar", u.UserName);
            Assert.AreEqual("OtherPassword", u.PasswordHash);
            Assert.AreEqual("Admin", u.Role);
            Assert.AreEqual(0, u.PublicKeys.Count);
        }

        [TestMethod]
        public void GetUserById_ShouldSucceed()
        {
            using (DbCommand insertTestData = memoryDbConnection.CreateCommand())
            {
                string keydata = Convert.ToBase64String(Encoding.Default.GetBytes("data"));
                insertTestData.CommandText = "INSERT INTO Users ('Username', 'PasswordHash', 'RoleID') VALUES " +
                                             "('FooUser', 'SecretPassword', 1)," +
                                             "('BarUser', 'OtherPassword', 2);" +
                                             "INSERT INTO PublicKeys ('UserID', 'PublicKeyNumber', 'KeyData', 'Active') VALUES " +
                                            $"(1, 1, '{keydata}', 1);";
                insertTestData.ExecuteNonQuery();
            }

            User fooUser = repository.GetById(1);

            Assert.AreEqual(1, fooUser.Id);
            Assert.AreEqual("FooUser", fooUser.UserName);
            Assert.AreEqual("FooUser".ToUpper(), fooUser.NormalizedUserName);
            Assert.AreEqual("SecretPassword", fooUser.PasswordHash);
            Assert.AreEqual("User", fooUser.Role);
            Assert.AreEqual(1, fooUser.PublicKeys.Count);
            Assert.AreEqual(1, fooUser.PublicKeys[0].Id);
            Assert.AreEqual(Key.KeyFlag.ACTIVE, fooUser.PublicKeys[0].Flag);
            Assert.AreEqual(Convert.ToBase64String(Encoding.Default.GetBytes("data")), Convert.ToBase64String(fooUser.PublicKeys[0].KeyData));

            User barUser = repository.GetById(2);

            Assert.AreEqual(2, barUser.Id);
            Assert.AreEqual("BarUser", barUser.UserName);
            Assert.AreEqual("BarUser".ToUpper(), barUser.NormalizedUserName);
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
                insertTestData.CommandText = "INSERT INTO Users ('Username', 'PasswordHash', 'RoleID') VALUES ('BarUser', 'pw', 1);";
                insertTestData.ExecuteNonQuery();
            }

            using (IAddUnitOfWork<User> uow = repository.Add())
            {
                uow.Entity.UserName = "FooUser";
                uow.Entity.NormalizedUserName = uow.Entity.UserName.Normalize();
                uow.Entity.PasswordHash = "SecretPassword";
                uow.Entity.Role = "User";
                uow.Entity.PublicKeys.Add(new Key() { 
                    KeyData = Encoding.Default.GetBytes("keydata1"),
                    Flag = Key.KeyFlag.ACTIVE
                });
                uow.Entity.PublicKeys.Add(new Key()
                {
                    KeyData = Encoding.Default.GetBytes("keydata2"),
                    Flag = Key.KeyFlag.OBSOLET
                });

                uow.Complete();
            }

            User fooUser = repository.GetById(2);

            Assert.AreEqual(2, fooUser.Id);
            Assert.AreEqual("FooUser", fooUser.UserName);
            Assert.AreEqual("FooUser".ToUpper(), fooUser.NormalizedUserName);
            Assert.AreEqual("SecretPassword", fooUser.PasswordHash);
            Assert.AreEqual("User", fooUser.Role);
            Assert.AreEqual(2, fooUser.PublicKeys.Count);
            Assert.AreEqual(1, fooUser.PublicKeys[0].Id);
            Assert.AreEqual(Key.KeyFlag.ACTIVE, fooUser.PublicKeys[0].Flag);
            Assert.AreEqual(Convert.ToBase64String(Encoding.Default.GetBytes("keydata1")), Convert.ToBase64String(fooUser.PublicKeys[0].KeyData));
            Assert.AreEqual(2, fooUser.PublicKeys[1].Id);
            Assert.AreEqual(Key.KeyFlag.OBSOLET, fooUser.PublicKeys[1].Flag);
            Assert.AreEqual(Convert.ToBase64String(Encoding.Default.GetBytes("keydata2")), Convert.ToBase64String(fooUser.PublicKeys[1].KeyData));
        }

        [TestMethod]
        public void AddUser_UsernameAlreadyExists_ShouldNotThrowException()
        {
            using (DbCommand inserTestData = memoryDbConnection.CreateCommand())
            {
                inserTestData.CommandText = "INSERT INTO Users('Username', 'PasswordHash', 'RoleID') VALUES ('BarUser', 'pw', 1);";
                inserTestData.ExecuteNonQuery();
            }

            using (IAddUnitOfWork<User> uow = repository.Add())
            {
                uow.Entity.UserName = "BarUser";
                uow.Entity.NormalizedUserName = uow.Entity.UserName.Normalize();
                uow.Entity.PasswordHash = "pw";
                uow.Entity.Role = "User";

                uow.Complete();
            }
        }

        [TestMethod]
        public void RemoveUser_ShouldSucceed()
        {
            using (DbCommand inserTestData = memoryDbConnection.CreateCommand())
            {
                inserTestData.CommandText = "INSERT INTO Users('Username', 'PasswordHash', 'RoleID') VALUES ('BarUser', 'pw', 1);" +
                                            "INSERT INTO PublicKeys ('UserID', 'PublicKeyNumber', 'KeyData', 'Active') VALUES (1, 1, 'base64string', 1);";
                inserTestData.ExecuteNonQuery();
            }

            using (IRemoveUnitOfWork<User, int> uow = repository.Remove(1))
            {
                uow.Complete();
            }

            using (DbCommand queryData = memoryDbConnection.CreateCommand())
            {
                queryData.CommandText = "SELECT * FROM Users WHERE Users.Username='BarUser';";
                using (IDataReader reader = queryData.ExecuteReader())
                {
                    Assert.IsFalse(reader.Read());
                }
                queryData.CommandText = "SELECT * FROM PublicKeys WHERE PublicKeys.UserID=1;";
                using (IDataReader reader = queryData.ExecuteReader())
                {
                    Assert.IsFalse(reader.Read());
                }
            }
        }

        [TestMethod]
        public void RemoveNotExistingUser_ShouldNotThrowException()
        {
            using (IRemoveUnitOfWork<User, int> uow = repository.Remove(99))
            {
                uow.Complete();
            }
        }

        [TestMethod]
        public void UpdateUser_ShouldSucceed()
        {
            using (DbCommand inserTestData = memoryDbConnection.CreateCommand())
            {
                inserTestData.CommandText = "INSERT INTO Users('Username', 'PasswordHash', 'RoleID') VALUES ('BarUser', 'pw', 1);" +
                                            "INSERT INTO PublicKeys ('UserID', 'PublicKeyNumber', 'KeyData', 'Active') VALUES (1, 1, 'base64string', 1);";
                inserTestData.ExecuteNonQuery();
            }

            using (IUpdateUnitOfWork<User, int> uow = repository.Update(1))
            {
                uow.Entity.UserName = "FooUser";
                uow.Entity.PasswordHash = "newPassword";
                uow.Entity.PublicKeys[0].Flag = Key.KeyFlag.OBSOLET;
                uow.Entity.PublicKeys[0].KeyData = Encoding.Default.GetBytes("ChangedKeyData");
                uow.Entity.Role = "Admin";
                uow.Complete();
            }

            User u = repository.GetById(1);
            Assert.AreEqual("FooUser", u.UserName);
            Assert.AreEqual("FooUser".ToUpper(), u.NormalizedUserName);
            Assert.AreEqual("newPassword", u.PasswordHash);
            Assert.AreEqual("Admin", u.Role);
            Assert.AreEqual(Key.KeyFlag.OBSOLET, u.PublicKeys[0].Flag);
            Assert.AreEqual(Convert.ToBase64String(Encoding.Default.GetBytes("ChangedKeyData")), Convert.ToBase64String(u.PublicKeys[0].KeyData));
        }

        [TestMethod]
        public void UpdateUser_AddNewKey_ShouldIgnore()
        {
            using (DbCommand inserTestData = memoryDbConnection.CreateCommand())
            {
                inserTestData.CommandText = "INSERT INTO Users('Username', 'PasswordHash', 'RoleID') VALUES ('BarUser', 'pw', 1);" +
                                            "INSERT INTO PublicKeys ('UserID', 'PublicKeyNumber', 'KeyData', 'Active') VALUES (1, 1, 'base64string', 1);";
                inserTestData.ExecuteNonQuery();
            }

            using (IUpdateUnitOfWork<User, int> uow = repository.Update(1))
            {
                uow.Entity.PublicKeys.Add(new Key()
                {
                    Id = 2,
                    KeyData = Encoding.Default.GetBytes("NewKeyData"),
                    Flag = Key.KeyFlag.ACTIVE
                });
                uow.Complete();
            }

            User u = repository.GetById(1);
            Assert.AreEqual(1, u.PublicKeys.Count);
        }

        [TestMethod]
        public void UpdateNotExistingUser_ShouldNotThrowException()
        {
            using (IUpdateUnitOfWork<User, int> uow = repository.Update(99))
            {
                uow.Complete();
            }
        }
    }
}
