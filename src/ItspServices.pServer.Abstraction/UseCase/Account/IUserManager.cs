using System.Threading.Tasks;

namespace ItspServices.pServer.Abstraction.UseCase.Account
{
    public interface IUserManager
    {
        Task<bool> CreateUserAsync(string username, string password);
    }
}
