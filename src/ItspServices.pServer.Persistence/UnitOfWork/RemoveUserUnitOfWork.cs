using System.Linq;
using System.Xml.Linq;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.UnitOfWork
{
    class RemoveUserUnitOfWork : IRemoveUnitOfWork<User>
    {
        static object _lockObject = new object();
        private string _filePath;
        bool _isCompleted;

        public int? Id { get; set; }

        public RemoveUserUnitOfWork(string filePath)
        {
            Id = null;
            _filePath = filePath;
            _isCompleted = false;
        }

        public void Complete()
        {
            if (_isCompleted)
                return;

            lock (_lockObject)
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
