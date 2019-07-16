using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

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
