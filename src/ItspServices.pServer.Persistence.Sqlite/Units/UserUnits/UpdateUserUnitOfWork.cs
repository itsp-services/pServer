using System.Data.Common;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.Sqlite.Units.UserUnits
{
    class UpdateUserUnitOfWork : SqliteUnitOfWork<User>, IUpdateUnitOfWork<User, int>
    {
        public int Id { get; internal set; }
        public User Entity { get; private set; }

        public UpdateUserUnitOfWork(DbProviderFactory dbFactory, string connectionString, User userToUpdate) 
            : base(dbFactory, connectionString)
        {
            Entity = userToUpdate;
        }

        protected override void Complete(DbConnection con)
        {
            using (DbCommand update = con.CreateCommand())
            {
                update.AddParameterWithValue("id", Id);
                update.AddParameterWithValue("username", Entity.UserName);
                update.AddParameterWithValue("pw", Entity.PasswordHash);
                update.CommandText = "UPDATE Users SET " +
                                     "Username=@username, PasswordHash=@pw " +
                                     "WHERE Users.ID=@id;";
                update.ExecuteNonQuery();
            }
        }

        public override void Dispose()
        {
            Entity = null;
        }
    }
}
