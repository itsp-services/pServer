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


        public bool Complete()
        {
            _users.Add(Entity);
            return true;
        }

        public void Dispose()
        {
            
        }
    }
}
