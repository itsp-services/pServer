using System.Data.Common;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.Sqlite.Units.UserUnits
{
    class RemoveUserUnitOfWork : SqliteUnitOfWork<User>, IRemoveUnitOfWork<User, int>
    {
        public int Id { get; internal set; }
        public User Entity { get; private set; }

        public RemoveUserUnitOfWork(DbProviderFactory dbFactory, string connectionString) 
            : base(dbFactory, connectionString)
        {
            Entity = new User();
        }

        protected override void Complete(DbConnection con)
        {
            using (DbCommand delete = con.CreateCommand())
            {
                delete.AddParameterWithValue("id", Id);
                delete.CommandText = "DELETE FROM Users WHERE Users.ID=@id";
                delete.ExecuteNonQuery();
            }
        }

        public override void Dispose()
        {
            Entity = null;
        }
    }
}
