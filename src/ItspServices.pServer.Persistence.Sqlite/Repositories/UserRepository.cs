using System;
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

        public UserRepository(DbProviderFactory dbFactory, string connectionString, ICollection<string> serverRoles)
        {
            _dbFactory = dbFactory;
            _connectionString = connectionString;

            InitRoles(serverRoles);
        }

        private void InitRoles(ICollection<string> serverRoles)
        {
            using (DbConnection con = _dbFactory.CreateAndOpenConnection(_connectionString))
            {
                using (DbCommand insert = con.CreateCommand())
                {
                    int i = 0;
                    insert.CommandText = "INSERT OR IGNORE INTO Roles(name) VALUES";
                    foreach (string role in serverRoles)
                    {
                        insert.AddParameterWithValue($"role{i}", role);
                        insert.CommandText += (i < serverRoles.Count - 1) ? $"(@role{i})," : $"(@role{i})";
                        i++;
                    }
                    insert.ExecuteNonQuery();
                }
            }
        }

        public User GetUserByNormalizedName(string name)
        {
            User user = new User();
            using (DbConnection con = _dbFactory.CreateAndOpenConnection(_connectionString))
            {
                using (DbCommand query = con.CreateCommand())
                {
                    query.AddParameterWithValue("searchedUsername", name);
                    query.CommandText = "SELECT * FROM [Users Keys] WHERE NormalizedUsername=@searchedUsername;";
                    using (IDataReader reader = query.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user.Id = reader.GetInt32(0);
                            user.UserName = reader.GetString(1);
                            user.NormalizedUserName = reader.GetString(2);
                            user.PasswordHash = reader.GetString(3);
                            user.Role = reader.GetString(4);
                            if (!reader.IsDBNull(5))
                            {
                                user.PublicKeys.Add(new Key()
                                {
                                    Id = reader.GetInt32(5),
                                    KeyData = Convert.FromBase64String(reader.GetString(6)),
                                    Flag = reader.GetBoolean(7) ? Key.KeyFlag.ACTIVE : Key.KeyFlag.OBSOLET
                                });
                            }
                        }
                        while(reader.Read())
                        {
                            user.PublicKeys.Add(new Key()
                            {
                                Id = reader.GetInt32(5),
                                KeyData = Convert.FromBase64String(reader.GetString(6)),
                                Flag = reader.GetBoolean(7) ? Key.KeyFlag.ACTIVE : Key.KeyFlag.OBSOLET
                            });
                        }
                    }
                }
            }
            return user;
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
            using (DbConnection con = _dbFactory.CreateAndOpenConnection(_connectionString))
            {
                using (DbCommand selectUser = con.CreateCommand())
                {
                    selectUser.CommandText = "SELECT Users.ID, Users.Username, Users.NormalizedUsername, Users.PasswordHash, Roles.Name AS Role FROM Users " +
                                             "INNER JOIN Roles ON Users.RoleID=Roles.ID " +
                                            $"WHERE Users.ID={id};";
                    using (IDataReader reader = selectUser.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User();
                            user.Id = reader.GetInt32(0);
                            user.UserName = reader.GetString(1);
                            user.NormalizedUserName = reader.GetString(2);
                            user.PasswordHash = reader.GetString(3);
                            user.Role = reader.GetString(4);
                        }
                    }
                }
            }
            return user;
        }

        private void AddPublicKeys(User user)
        {
            using (DbConnection con = _dbFactory.CreateAndOpenConnection(_connectionString))
            {
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
                            k.KeyData = Convert.FromBase64String(reader.GetString(1));
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
            return new UpdateUserUnitOfWork(_dbFactory, _connectionString, GetById(key))
            {
                Id = key
            };
        }

        public IUnitOfWork<Key> AddPublicKey(User user, Key key)
        {
            return new AddPublicKeyUnitOfWork(_dbFactory, _connectionString, user.Id, key);
        }
    }
}
