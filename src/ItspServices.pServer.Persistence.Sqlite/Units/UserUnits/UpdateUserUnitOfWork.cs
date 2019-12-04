using System.Data.Common;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.Sqlite.Units.UserUnits
{
    class UpdateUserUnitOfWork : SqliteUnitOfWork<User>, IUpdateUnitOfWork<User, int>
    {
        private int _numberOfKeys;
        public int Id { get; internal set; }
        public User Entity { get; private set; }

        public UpdateUserUnitOfWork(DbProviderFactory dbFactory, string connectionString, User userToUpdate)
            : base(dbFactory, connectionString)
        {
            Entity = userToUpdate;
            _numberOfKeys = Entity?.PublicKeys.Count ?? 0;
        }

        protected override void Complete(DbConnection con)
        {
            if (Entity == null)
                return;

            using (DbCommand update = con.CreateCommand())
            {
                update.AddParameterWithValue("id", Id);
                update.AddParameterWithValue("username", Entity.UserName);
                update.AddParameterWithValue("pw", Entity.PasswordHash);
                update.AddParameterWithValue("role", Entity.Role);
                update.CommandText = "UPDATE Users " +
                                     "SET Username=@username, PasswordHash=@pw, RoleID=(SELECT ID FROM Roles WHERE Roles.Name=@role) " +
                                     "WHERE Users.ID=@id;";
                for (int i = 0; i < _numberOfKeys && i < Entity.PublicKeys.Count; i++)
                {
                    int active = (Entity.PublicKeys[i].Flag == Key.KeyFlag.ACTIVE) ? 1 : 0;
                    update.AddParameterWithValue($"keydata{Entity.PublicKeys[i].Id}", Entity.PublicKeys[i].AsBase64String());
                    update.AddParameterWithValue($"keyID{Entity.PublicKeys[i].Id}", Entity.PublicKeys[i].Id);
                    update.CommandText += $"UPDATE PublicKeys SET " +
                                          $"KeyData=@keydata{Entity.PublicKeys[i].Id}, " +
                                          $"Active={active} " +
                                          $"WHERE PublicKeyNumber=@keyID{Entity.PublicKeys[i].Id} AND UserID=@id;";
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
