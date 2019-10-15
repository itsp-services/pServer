using System.Data.Common;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.Sqlite.Units
{
    abstract class SqliteUnitOfWork<T> : IUnitOfWork<T>
    {
        private string _connectionString;
        protected DbProviderFactory dbFactory;

        public SqliteUnitOfWork(DbProviderFactory dbFactory, string connectionString)
        {
            this.dbFactory = dbFactory;
            _connectionString = connectionString;
        }

        void IUnitOfWork<T>.Complete()
        {
            using (DbConnection con = dbFactory.CreateConnection())
            {
                con.ConnectionString = _connectionString;
                con.Open();
                this.Complete(con);
            }
        }

        protected abstract void Complete(DbConnection con);

        public abstract void Dispose();
    }
}
