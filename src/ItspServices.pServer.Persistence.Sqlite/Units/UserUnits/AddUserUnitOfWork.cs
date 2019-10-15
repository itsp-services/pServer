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
            int roleId = 0;
            using (DbCommand queryRole = con.CreateCommand())
            {
                queryRole.CommandText = "SELECT ID FROM Roles " +
                                       $"WHERE Roles.Name='{Entity.Role}';";
                using (IDataReader reader = queryRole.ExecuteReader())
                {
                    if (reader.Read())
                        roleId = reader.GetInt32(0);
                    else
                        return;
                }
            }

            using (DbCommand insert = con.CreateCommand())
            {
                insert.CommandText = "INSERT INTO Users ('Username', 'PasswordHash', 'RoleID') VALUES" +
                                        $"('{Entity.UserName}', '{Entity.PasswordHash}', {roleId});";
                insert.ExecuteNonQuery();
                insert.CommandText = "INSERT INTO PublicKeys ('UserID', 'PublicKeyNumber', 'KeyData', 'Active) VALUES ";
                foreach (Key key in Entity.PublicKeys)
                {
                    // TODO: Insert keys
                }
            }
        }

        public override void Dispose()
        {

        }

    }
}
