using System.Data.Common;
using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Persistence.Sqlite.Units.UserUnits
{
    class AddPublicKeyUnitOfWork : SqliteUnitOfWork<Key>
    {
        private int _userId;
        private Key _publicKey;

        public AddPublicKeyUnitOfWork(DbProviderFactory dbFactory, string connectionString, int id, Key publicKey) 
            : base(dbFactory, connectionString)
        {
            _publicKey = publicKey;
            _userId = id;
        }

        protected override void Complete(DbConnection con)
        {
            using (DbCommand insert = con.CreateCommand())
            {
                insert.AddParameterWithValue("userId", _userId);
                insert.AddParameterWithValue("keydata", _publicKey.AsBase64String());
                insert.AddParameterWithValue("active", (_publicKey.Flag == Key.KeyFlag.ACTIVE) ? 1 : 0);
                insert.CommandText = "INSERT INTO PublicKeys(UserID, PublicKeyNumber, KeyData, Active) VALUES (@userId, -1, @keydata, @active);";
                insert.ExecuteNonQuery();
            }
        }

        public override void Dispose()
        {
            _publicKey = null;
        }
    }
}
