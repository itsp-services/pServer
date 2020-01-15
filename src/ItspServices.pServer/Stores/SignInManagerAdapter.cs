using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.UseCase.Account;

namespace ItspServices.pServer.Stores
{
    public class SignInManagerAdapter : ISignInManager
    {
        private readonly SignInManager<User> _signInManager;

        public SignInManagerAdapter(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<bool> PasswordSignInAsync(string username, string password)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(username, password, false, false);
            return result.Succeeded;
        }

        public async Task<bool> SignOutAsync()
        {
            await _signInManager.SignOutAsync();
            return true;
        }
    }
}
