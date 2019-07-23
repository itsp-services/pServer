using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByNormalizedName(string name);
    }
}
