using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Test.Mock.Units
{
    class MockRemoveUserUnit : IUnitOfWork<User>
    {
        private List<User> _users;

        public User Entity { get; }

        public MockRemoveUserUnit(List<User> users)
        {
            _users = users;
        }

        public bool Complete()
        {
            _users.Remove(Entity);
            return true;
        }

        public void Dispose()
        {
            
        }
    }
}
