using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Test.Mock.Units
{
    class MockUpdateUserUnit : IUnitOfWork<User>
    {
        private User _userToUpdate;

        public User Entity { get; }

        public MockUpdateUserUnit(User userToUpdate)
        {
            _userToUpdate = userToUpdate;
            Entity = new User();
        }

        public bool Complete()
        {
            if (_userToUpdate.Id != Entity.Id)
                return false;
            _userToUpdate = Entity;
            return true;
        }

        public void Dispose()
        {

        }
    }
}
