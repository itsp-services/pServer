using ItspServices.pServer.Abstraction.Repository;
using Microsoft.Extensions.Options;

namespace ItspServices.pServer.Persistence.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        public RepositoryManager(IOptions<PersistenceOption> option)
        {
            UserRepository = new UserRepository(option.Value.Path);
        }

        public IUserRepository UserRepository { get; }

        public IProtectedDataRepository ProtectedDataRepository { get; }
    }
}
