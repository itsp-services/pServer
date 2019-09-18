using System;
using System.Linq;
using System.Xml.Linq;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence
{
    class UpdateUserUnitOfWork : IUpdateUnitOfWork<User>
    {
        static readonly object _lockObject = new object();
        private string _filePath;
        bool _isCompleted;

        public int Id { get; }

        public User Entity { get; }

        public UpdateUserUnitOfWork(string filePath, User user)
        {
            Entity = user;
            Id = user.Id;
            _filePath = filePath;
            _isCompleted = false;
        }

        public void Complete()
        {
            if (Entity.Id != Id)
                throw new InvalidOperationException("Change of the Id to update is not supported.");

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
