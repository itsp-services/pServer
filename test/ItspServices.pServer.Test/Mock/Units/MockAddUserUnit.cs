using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Test.Mock.Units
{
    class MockAddUserUnit : IUserUnit
    {
        private List<User> _users;

        public User User { get; }

        public MockAddUserUnit(List<User> users)
        {
            _users = users;
        }

        public void Complete()
        {
            _users.Add(User);
        }

        public void Dispose()
        {
            
        }
    }
}
