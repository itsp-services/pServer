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
                // TODO: Use DbParameter for user input
                insert.CommandText = "INSERT INTO Users(Username, PasswordHash, RoleID) " +
                                    $"SELECT N, Pw, ID FROM(SELECT '{Entity.UserName}' AS N, '{Entity.PasswordHash}' AS Pw) " +
                                    $"JOIN Roles ON Roles.Name='{Entity.Role}';";
                // TODO: Add Keys
                insert.ExecuteNonQuery();
            }
        }

        public override void Dispose()
        {

        }

    }
}
