using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Persistence.Sqlite.Repositories;
using Microsoft.Data.Sqlite;

namespace ItspServices.pServer.ServerTest.Persistence.SqliteTests
{
    [TestClass]
    public class UserRepositoryTests
    {
        UserRepository repository;

        #region Initialize and cleanup methods

        [TestInitialize]
        public void Init()
        {
            repository = new UserRepository(SqliteFactory.Instance, "Data Source=pServer; Mode=Memory; Cache=Shared");
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
