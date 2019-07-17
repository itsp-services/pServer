using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Test.Mock.Units
{
    class MockUpdateUserUnit : IUnitOfWork<User>
    {
        private List<User> _users;

        public User Entity { get; }

        public MockUpdateUserUnit(List<User> users)
        {
            _users = users;
            Entity = new User();
        }

        public void Complete()
        {
            User userToUpdate = _users.Find(user => user.Id == Entity.Id);
            userToUpdate = Entity;
        }

        public void Dispose()
        {

        }
    }
}
