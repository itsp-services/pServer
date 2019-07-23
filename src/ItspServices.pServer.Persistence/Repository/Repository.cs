using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using Microsoft.Extensions.Options;

namespace ItspServices.pServer.Persistence.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        public RepositoryManager(IOptions<PersistenceOption> option)
        {

        }

        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public IProtectedDataRepository ProtectedDataRepository { get; }

        public RepositoryManager()
        {
            UserRepository = new UserRepository("");
            RoleRepository = new RoleRepository();
            
        }
    }
}
