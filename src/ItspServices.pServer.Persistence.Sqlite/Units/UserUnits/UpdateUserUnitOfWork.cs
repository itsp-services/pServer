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
            if(Entity == null)
                return;

            using (DbCommand update = con.CreateCommand())
            {
                update.AddParameterWithValue("id", Id);
                update.AddParameterWithValue("username", Entity.NormalizedUserName);
                update.AddParameterWithValue("pw", Entity.PasswordHash);
                update.AddParameterWithValue("role", Entity.Role);
                update.CommandText = "UPDATE Users " +
                                     "SET Username=@username, PasswordHash=@pw, RoleID=(SELECT ID FROM Roles WHERE Roles.Name=@role) " +
                                     "WHERE Users.ID=@id;";
                if (Entity.HasKeys())
                {
                    foreach (Key key in Entity.PublicKeys)
                    {
                        int active = (key.Flag == Key.KeyFlag.ACTIVE) ? 1 : 0;
                        update.AddParameterWithValue($"keydata{key.Id}", key.AsBase64String());
                        update.AddParameterWithValue($"keyID{key.Id}", key.Id);
                        update.CommandText += $"UPDATE PublicKeys SET KeyData=@keydata{key.Id}, Active={active} WHERE PublicKeyNumber=@keyID{key.Id} AND UserID=@id;";
                    }
                }
                update.ExecuteNonQuery();
            }
        }

        public override void Dispose()
        {
            Entity = null;
        }
    }
}
