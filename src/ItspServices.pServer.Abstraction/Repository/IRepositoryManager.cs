using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }
        IProtectedDataRepository ProtectedDataRepository { get; }
    }
}
