using ItspServices.pServer.Abstraction.Repository;
using Microsoft.Extensions.Options;

namespace ItspServices.pServer.Persistence.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        public RepositoryManager(IOptions<PersistenceOption> option)
        {
            UserRepository = new UserRepository(option.Value.Path);
            RoleRepository = new RoleRepository();
        }

        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public IProtectedDataRepository ProtectedDataRepository { get; }
    }
}
