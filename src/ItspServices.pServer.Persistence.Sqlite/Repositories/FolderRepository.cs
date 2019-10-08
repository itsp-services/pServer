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
        }

        public Folder GetById(int id)
        {

            return new Folder() {
                Id = 0,
                Name = "root",
                ParentId = 0,
                Subfolder = new List<Folder>(),
                DataRegister = new List<ProtectedData>()
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
