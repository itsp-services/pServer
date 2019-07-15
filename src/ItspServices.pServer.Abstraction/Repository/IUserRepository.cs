using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IUserRepository : IRepositoryPart<User>
    {
        User GetUserByName(string name);
    }
}
