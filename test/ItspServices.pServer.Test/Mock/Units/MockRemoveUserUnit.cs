using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Units;
using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Test.Mock.Units
{
    class MockRemoveUserUnit : IUserUnit
    {
        private List<User> _users;

        public User User { get; }

        public MockRemoveUserUnit(List<User> users)
        {
            _users = users;
        }

        public void Complete()
        {
            _users.Remove(User);
        }

        public void Dispose()
        {
            
        }
    }
}
