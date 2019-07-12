
using ItspServices.pServer.Abstraction.Models;
using System.Threading.Tasks;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IUserRepository : IRepositoryPart<User>
    {
        Task<User> GetUserByNameAsync(string name);
    }
}
