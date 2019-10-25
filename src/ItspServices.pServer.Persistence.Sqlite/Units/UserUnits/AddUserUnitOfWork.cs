using System;
using System.Data;
using System.Data.Common;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.Sqlite.Units.UserUnits
{
    class AddUserUnitOfWork : SqliteUnitOfWork<User>, IAddUnitOfWork<User>
    {
        public User Entity { get; }

        public AddUserUnitOfWork(DbProviderFactory sqlFactory, string connectionString)
            : base(sqlFactory, connectionString)
        {
            Entity = new User();
        }

        protected override void Complete(DbConnection con)
        {
            using (DbCommand insert = con.CreateCommand())
            {
                insert.AddParameterWithValue("username", Entity.UserName);
                insert.AddParameterWithValue("password", Entity.PasswordHash);
                insert.AddParameterWithValue("role", Entity.Role);

                insert.CommandText = "INSERT INTO Users(Username, PasswordHash, RoleID) " +
                                    $"SELECT N, Pw, ID FROM(SELECT @username AS N, @password AS Pw) " +
                                    $"JOIN Roles ON Roles.Name=@role;";
                insert.ExecuteNonQuery();

                if (Entity.PublicKeys.Count > 0)
                {
                    insert.CommandText = $"SELECT ID FROM Users WHERE Users.UserName=@username;";
                    int userID = -1;
                    using (IDataReader reader = insert.ExecuteReader())
                    {
                        reader.Read();
                        userID = reader.GetInt32(0);
                    }
                    insert.CommandText = "INSERT INTO PublicKeys ('UserID', 'PublicKeyNumber', 'KeyData', 'Active') VALUES ";
                    for (int i = 0; i < Entity.PublicKeys.Count; i++)
                    {
                        insert.AddParameterWithValue($"keydata{i}", Convert.ToBase64String(Entity.PublicKeys[i].KeyData));

                        int active = (Entity.PublicKeys[i].Flag == Key.KeyFlag.ACTIVE) ? 1 : 0;
                        insert.CommandText += $"({userID}, {i + 1}, @keydata{i}, {active})";
                        if (i >= Entity.PublicKeys.Count - 1)
                        {
                            insert.CommandText += ';';
                        }
                        else
                        {
                            insert.CommandText += ',';
                        }
                    }
                    insert.ExecuteNonQuery();
                }
            }
        }

        public override void Dispose()
        {

        }

    }
}
