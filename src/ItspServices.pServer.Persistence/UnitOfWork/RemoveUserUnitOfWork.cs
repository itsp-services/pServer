using System.Linq;
using System.Xml.Linq;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.UnitOfWork
{
    class RemoveUserUnitOfWork : IRemoveUnitOfWork<User>
    {
        static readonly object _lockObject = new object();
        string _filePath;
        bool _isCompleted;

        public int Id { get; }

        public User Entity { get; }

        public RemoveUserUnitOfWork(string filePath, User entity)
        {
            _filePath = filePath;
            _isCompleted = false;
            Id = entity.Id;
            Entity = entity;
        }

        public void Complete()
        {
            if (_isCompleted)
                return;

            lock (LockObject.Lock)
            {
                XDocument document = XDocument.Load(_filePath);
                document.Root.Elements()
                             .Where(x => (int)x.Attribute("Id") == Id)
                             .Remove();
                document.Save(_filePath);
                _isCompleted = true;
            }
        }

        public void Dispose()
        {
            _isCompleted = true;
            _filePath = null;
        }
    }
}
