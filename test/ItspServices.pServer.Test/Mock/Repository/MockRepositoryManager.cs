using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using System;

namespace ItspServices.pServer.Test.Mock.Repository
{
    class MockRepositoryManager : IRepositoryManager
    {
        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public IRepository<ProtectedData> ProtectedDataRepository { get => throw new NotImplementedException(); }

        public MockRepositoryManager()
        {
            UserRepository = new MockUserRepository();
            RoleRepository = new MockRoleRepository();
        }
    }
}
