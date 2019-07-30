using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using System;

namespace ItspServices.pServer.Test.Mock.Repository
{
    class MockRepositoryManager : IRepositoryManager
    {
        public IUserRepository UserRepository { get; }

        public IProtectedDataRepository ProtectedDataRepository { get; }

        public MockRepositoryManager()
        {
            UserRepository = new MockUserRepository();
            ProtectedDataRepository = new MockProtectedDataRepository();
        }
    }
}
