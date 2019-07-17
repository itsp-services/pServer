using ItspServices.pServer.Abstraction.Repository;

namespace ItspServices.pServer.Persistence.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public RepositoryManager()
        {
            UserRepository = new UserRepository();
            RoleRepository = new RoleRepository();
        }
    }
}
