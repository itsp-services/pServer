using System.Collections.Generic;
using System.Text;
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
            Folder sampleSubfolder = new Folder()
            {
                Id = 1,
                Name = "foo"
            };
            rootFolder.Subfolder.Add(sampleSubfolder);
            _folders.Add(rootFolder);
            _folders.Add(sampleSubfolder);

            ProtectedData sampleData1 = new ProtectedData()
            {
                Id = 0,
                OwnerId = 0,
                Name = "FooData"
            };
            sampleData1.Data = Encoding.UTF8.GetBytes("BarData");
            _data.Add(sampleData1);

            ProtectedData sampleData2 = new ProtectedData()
            {
                Id = 1,
                OwnerId = 1,
                Name = "FooData2"
            };
            _data.Add(sampleData2);
        }

        public IEnumerable<ProtectedData> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public ProtectedData GetById(int id)
        {
            return _data.Find(x => x.Id == id);
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
    }
}
