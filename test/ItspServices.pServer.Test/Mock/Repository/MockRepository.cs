using ItspServices.pServer.Abstraction.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Test.Mock.Repository
{
    class MockRepository : IRepository
    {
        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public MockRepository()
        {
            UserRepository = new MockUserRepository();
            RoleRepository = new MockRoleRepository();
        }
    }
}
