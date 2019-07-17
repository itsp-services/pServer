using ItspServices.pServer.Abstraction.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Test.Mock.Repository
{
    class MockRepositoryManager : IRepositoryManager
    {
        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public MockRepositoryManager()
        {
            UserRepository = new MockUserRepository();
            RoleRepository = new MockRoleRepository();
        }
    }
}
