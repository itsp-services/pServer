using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Test.Mock.Units
{
    class MockRemoveUserUnit : IUserUnit
    {
        private List<User> _users;

        public User User { get; set; }

        public MockRemoveUserUnit(List<User> users)
        {
            _users = users;
        }

        public bool Complete()
        {
            _users.Remove(User);
            return true;
        }

        public void Dispose()
        {
            
        }
    }
}
