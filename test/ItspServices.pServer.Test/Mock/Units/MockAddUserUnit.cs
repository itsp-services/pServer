using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Test.Mock.Units
{
    class MockAddUserUnit : IUserUnit
    {
        private List<User> _users;

        public User User { get; set; }

        public MockAddUserUnit(List<User> users)
        {
            _users = users;
            User = new User();
        }

        public bool Complete()
        {
            _users.Add(User);
            return true;
        }

        public void Dispose()
        {
            
        }
    }
}
