using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;
using System;
using System.Collections.Generic;

namespace ItspServices.pServer.Test.Mock.Units
{
    class MockUpdateUserUnit : IUserUnit
    {
        private List<User> _users;

        public User User { get; }

        public MockUpdateUserUnit(List<User> users)
        {
            
        }

        public void Complete()
        {
            User userToUpdate = _users.Find(u => u.Id == User.Id);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
