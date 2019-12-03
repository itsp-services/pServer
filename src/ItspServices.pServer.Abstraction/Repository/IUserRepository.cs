using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IUserRepository : IRepository<User, int>
    {
        User GetUserByNormalizedName(string name);
        IUnitOfWork<Key> AddPublicKey(User user, Key key);
    }
}
