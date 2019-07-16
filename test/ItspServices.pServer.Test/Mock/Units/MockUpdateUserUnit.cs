using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Test.Mock.Units
{
    class MockUpdateUserUnit : IUserUnit
    {
        private User _userToUpdate;

        public User User { get; set; }

        public MockUpdateUserUnit(User userToUpdate)
        {
            _userToUpdate = userToUpdate;
            User = new User();
        }

        public bool Complete()
        {
            if (_userToUpdate.Id != User.Id)
                return false;
            _userToUpdate = User;
            return true;
        }

        public void Dispose()
        {

        }
    }
}
