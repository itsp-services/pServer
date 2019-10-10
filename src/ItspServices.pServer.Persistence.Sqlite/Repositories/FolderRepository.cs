using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.Sqlite.Repositories
{
    class FolderRepository : IRepository<Folder, int>
    {

        private DbProviderFactory _sqlFactory;
        private readonly string _connectionString;

        public FolderRepository(DbProviderFactory sqlFactory, string connectionString)
        {
            _sqlFactory = sqlFactory;
            _connectionString = connectionString;
            InitRootFolder();
        }

        public void InitRootFolder()
        {
            using (DbConnection con = _sqlFactory.CreateConnection())
            {
                con.ConnectionString = _connectionString;
                con.Open();

                using (DbCommand insertRoot = con.CreateCommand())
                {
                    insertRoot.CommandText = "INSERT INTO Folders('ID', 'FolderName', 'Parent') VALUES " +
                                             "(0, 'root', 0);";
                    insertRoot.ExecuteNonQuery();
                }
            }
        }

        public Folder GetById(int id)
        {

            return new Folder() {
                Id = 0,
                Name = "root",
                ParentId = 0,
                SubfolderIds = new List<int>(),
                DataIds = new List<int>()
            };
        }

        public IEnumerable<Folder> GetAll()
        {
            throw new NotImplementedException();
        }

        public IAddUnitOfWork<Folder> Add()
        {
            throw new NotImplementedException();
        }

        public IRemoveUnitOfWork<Folder, int> Remove(int key)
        {
            throw new NotImplementedException();
        }

        public IUpdateUnitOfWork<Folder, int> Update(int key)
        {
            throw new NotImplementedException();
        }
    }
}
