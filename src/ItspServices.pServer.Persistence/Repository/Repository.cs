using System;
using ItspServices.pServer.Abstraction.Repository;

namespace ItspServices.pServer.Persistence.Repository
{
    public class Repository : IRepository
    {
        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public Repository()
        {
            UserRepository = new UserRepository();
            RoleRepository = new RoleRepository();
        }
    }
}
