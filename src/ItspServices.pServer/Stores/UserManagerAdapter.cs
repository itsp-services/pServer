using System.Threading.Tasks;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.UseCase.Account;
using Microsoft.AspNetCore.Identity;

namespace ItspServices.pServer.Stores
{
    public class UserManagerAdapter : IUserManager
    {
        private readonly UserManager<User> _userManager;

        public UserManagerAdapter(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> CreateUserAsync(string username, string password)
        {
            User user = new User()
            {
                UserName = username,
                NormalizedUserName = username.ToUpper()
            };
            IdentityResult result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }
    }
}
