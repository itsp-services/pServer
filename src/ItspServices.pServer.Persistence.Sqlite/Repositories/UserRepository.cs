using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;
using ItspServices.pServer.Persistence.Sqlite.Units.UserUnits;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ItspServices.pServer.ServerTest")]
namespace ItspServices.pServer.Persistence.Sqlite.Repositories
{
    class UserRepository : IUserRepository
    {
        private DbProviderFactory _dbFactory;
        private readonly string _connectionString;

        public UserRepository(DbProviderFactory dbFactory, string connectionString)
        {
            _dbFactory = dbFactory;
            _connectionString = connectionString;
        }

        public User GetUserByNormalizedName(string name)
        {
            throw new System.NotImplementedException();
        }

        public User GetById(int id)
        {
            User user = ReadUserData(id);
            if(user == null)
            {
                return null;
            }
            else
            {
                AddPublicKeys(user);
                return user;
            }
        }

        private User ReadUserData(int id)
        {
            User user = null;
            using (DbConnection con = _dbFactory.CreateConnection())
            {
                con.ConnectionString = _connectionString;
                con.Open();

                using (DbCommand selectUser = con.CreateCommand())
                {
                    selectUser.CommandText = "SELECT Users.ID, Users.Username, Users.PasswordHash, Roles.Name AS Role FROM Users " +
                                             "INNER JOIN Roles ON Users.RoleID=Roles.ID " +
                                            $"WHERE Users.ID={id};";
                    using (IDataReader reader = selectUser.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User();
                            user.Id = reader.GetInt32(0);
                            user.UserName = reader.GetString(1);
                            user.NormalizedUserName = user.UserName.Normalize();
                            user.PasswordHash = reader.GetString(2);
                            user.Role = reader.GetString(3);
                        }
                    }
                }
            }
            return user;
        }

        private void AddPublicKeys(User user)
        {
            using (DbConnection con = _dbFactory.CreateConnection())
            {
                con.ConnectionString = _connectionString;
                con.Open();

                using (DbCommand queryKeys = con.CreateCommand())
                {
                    queryKeys.CommandText = "SELECT PublicKeys.PublicKeyNumber, PublicKeys.KeyData, PublicKeys.Active FROM PublicKeys " +
                                           $"WHERE PublicKeys.UserID={user.Id};";
                    using (IDataReader reader = queryKeys.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Key k = new Key();
                            k.Id = reader.GetInt32(0);
                            k.KeyData = new byte[364];
                            reader.GetBytes(1, 0, k.KeyData, 0, 364);
                            k.Flag = reader.GetBoolean(2) ? Key.KeyFlag.ACTIVE : Key.KeyFlag.OBSOLET;
                            user.PublicKeys.Add(k);
                        }
                    }
                }
            }
        }

        public IEnumerable<User> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IAddUnitOfWork<User> Add()
        {
            return new AddUserUnitOfWork(_dbFactory, _connectionString);
        }

        public IRemoveUnitOfWork<User, int> Remove(int key)
        {
            return new RemoveUserUnitOfWork(_dbFactory, _connectionString)
            {
                Id = key
            };
        }

        public IUpdateUnitOfWork<User, int> Update(int key)
        {
            throw new System.NotImplementedException();
        }
    }
}
