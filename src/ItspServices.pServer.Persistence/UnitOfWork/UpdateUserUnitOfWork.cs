using System.Linq;
using System.Xml.Linq;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.UnitOfWork
{
    class UpdateUserUnitOfWork : IUpdateUnitOfWork<User>
    {
        static object _lockObject = new object();
        private string _filePath;
        bool _isCompleted;

        public int? Id { get; set; }

        public User Entity { get; }

        public UpdateUserUnitOfWork(string filePath)
        {
            Entity = new User();
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
                XElement elementToUpdate = (from userEntry in document.Descendants("User")
                                            where (int)userEntry.Attribute("Id") == Id
                                            select userEntry).SingleOrDefault();
                UserSerializer.UpdateXElement(elementToUpdate, Entity);

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
