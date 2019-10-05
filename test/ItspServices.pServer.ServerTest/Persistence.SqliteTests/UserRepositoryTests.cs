using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Persistence.Sqlite.Repositories;
using ItspServices.pServer.Persistence.Sqlite;
using System.Data.SQLite;
using System.Data.Common;

namespace ItspServices.pServer.ServerTest.Persistence.SqliteTests
{
    [TestClass]
    public class UserRepositoryTests
    {
        SqlUserRepository repository;

        #region Initialize and cleanup methods

        [TestInitialize]
        public void Init()
        {
            SqlOptions options = new SqlOptions() { ConnectionString = "Data Source=:memory:;Version=3;New=True;" };
            repository = new SqlUserRepository(new SQLiteFactory(), options);
        }

        [TestCleanup]
        public void Cleanup()
        {
            repository = null;
        }

        #endregion

        [TestMethod]
        public void GetUserById_ShouldSucceed()
        {
            
        }
    }
}
