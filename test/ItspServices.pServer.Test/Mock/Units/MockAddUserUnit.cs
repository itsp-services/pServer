using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Test.Mock.Units
{
    class MockAddUserUnit : IUnitOfWork<User>
    {
        private List<User> _users;

        public User Entity { get; }

        public MockAddUserUnit(List<User> users)
        {
            _users = users;
            Entity = new User();
        }

        public void Complete()
        {
            _users.Add(Entity);
        }

        public void Dispose()
        {
            
        }
    }
}
