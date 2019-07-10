using ItspServices.pServer.Repository;
using ItspServices.pServer.Abstraction.Repository;

namespace ItspServices.pServer.Persistence.Repository
{
    class Repository : IRepository
    {
        public IUserRepository UserPart { get; } = new UserRepository();
    }
}
