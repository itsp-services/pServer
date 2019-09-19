using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Persistence.Repository
{
    class ProtectedDataRepository : IProtectedDataRepository
    {
        public IAddUnitOfWork<ProtectedData> Add()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork<ProtectedData> AddToFolder(ProtectedData data, Folder folder)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProtectedData> GetAll()
        {
            throw new NotImplementedException();
        }

        public ProtectedData GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Folder GetFolderById(int? folderId)
        {
            throw new NotImplementedException();
        }

        public List<Folder> GetSubfolders(int? folderId)
        {
            throw new NotImplementedException();
        }

        public IRemoveUnitOfWork<ProtectedData> Remove(int key)
        {
            throw new NotImplementedException();
        }

        public IUpdateUnitOfWork<ProtectedData> Update(int key)
        {
            throw new NotImplementedException();
        }
    }
}
