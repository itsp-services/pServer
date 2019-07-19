using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Test.Mock.Repository
{
    public class MockProtectedDataRepository : IProtectedDataRepository
    {
        private const int ROOT_ID = 0;
        private static readonly Folder rootFolder = new Folder()
        {
            Id = ROOT_ID,
            Name = "root"
        };

        private List<ProtectedData> _data = new List<ProtectedData>();
        private List<Folder> _folders = new List<Folder>();



        public MockProtectedDataRepository()
        {
            _folders.Add(rootFolder);
        }

        public IEnumerable<ProtectedData> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public ProtectedData GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public IUnitOfWork<ProtectedData> Add(ProtectedData entity)
        {
            throw new System.NotImplementedException();
        }

        public IUnitOfWork<ProtectedData> Remove(ProtectedData entity)
        {
            throw new System.NotImplementedException();
        }

        public IUnitOfWork<ProtectedData> Update(ProtectedData entity)
        {
            throw new System.NotImplementedException();
        }

        public Folder GetFolderById(int? folderId)
        {
            return (folderId != null) ? _folders.Find(f => f.Id == folderId) : _folders.Find(f => f.Id == ROOT_ID);
        }

        public List<Folder> GetSubfolders(int? folderId)
        {
            return (folderId != null) ? _folders.Find(f => f.Id == folderId).Subfolder :
                _folders.Find(f => f.Id == ROOT_ID).Subfolder;
        }
    }
}
