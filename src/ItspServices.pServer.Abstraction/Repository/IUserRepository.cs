using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IUserRepository : IRepository<User, int>
    {
        User GetUserByNormalizedName(string name);
    }
}
