using System.Threading.Tasks;

namespace ItspServices.pServer.Abstraction.UseCase.Account
{
    public interface ISignInManager
    {
        Task<bool> PasswordSignInAsync(string username, string password);
        Task<bool> SignOutAsync();
    }
}
