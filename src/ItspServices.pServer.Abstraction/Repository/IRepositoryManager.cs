using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        IRepository<ProtectedData> ProtectedDataRepository { get; }
    }
}
