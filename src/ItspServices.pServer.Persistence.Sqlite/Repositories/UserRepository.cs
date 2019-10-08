using System.Collections.Generic;
using System.Data.Common;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ItspServices.pServer.ServerTest")]
namespace ItspServices.pServer.Persistence.Sqlite.Repositories
{
    class UserRepository : IUserRepository
    {
        private DbProviderFactory _sqlFactory;
        private readonly string _connectionString;

        public UserRepository(DbProviderFactory sqlFactory, string connectionString)
        {
            _sqlFactory = sqlFactory;
            _connectionString = connectionString;
        }

        public User GetUserByNormalizedName(string name)
        {
            throw new System.NotImplementedException();
        }

        public User GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IAddUnitOfWork<User> Add()
        {
            throw new System.NotImplementedException();
        }

        public IRemoveUnitOfWork<User, int> Remove(int key)
        {
            throw new System.NotImplementedException();
        }

        public IUpdateUnitOfWork<User, int> Update(int key)
        {
            throw new System.NotImplementedException();
        }
    }
}
