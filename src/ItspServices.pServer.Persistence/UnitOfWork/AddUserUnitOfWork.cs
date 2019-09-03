using System.Linq;
using System.Xml.Linq;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence
{
    class AddUserUnitOfWork : IAddUnitOfWork<User>
    {
        static object _lockObject = new object();
        string _filePath;
        bool _isCompleted;

        public AddUserUnitOfWork(string filePath)
        {
            _filePath = filePath;
            Entity = new User();
            _isCompleted = false;
        }

        public User Entity { get; }

        public void Complete()
        {
            if (_isCompleted)
                return;

            lock (_lockObject)
            {
                XDocument document = XDocument.Load(_filePath);
                Entity.Id = document.Descendants("User")
                    .Select(x => int.Parse(x.Attribute("Id").Value))
                    .OrderByDescending(x => x)
                    .FirstOrDefault() + 1;

                document.Element("Users").Add(UserSerializer.UserToXElement(Entity));
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
