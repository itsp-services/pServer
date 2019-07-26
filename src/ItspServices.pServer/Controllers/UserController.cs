using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ItspServices.pServer.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IRepositoryManager repositoryManager)
        {
            _userRepository = repositoryManager.UserRepository;
        }

        public IActionResult Index()
        {
            User user = _userRepository.GetUserByNormalizedName(User.Identity.Name.ToUpper());
            return View(user);
        }

        [HttpPost]
        public void Index([FromForm]string newKey)
        {
            if (newKey == null)
                return;
            User user = _userRepository.GetUserByNormalizedName(User.Identity.Name.ToUpper());
            user.PublicKeys.Add(new Key() { KeyData = Encoding.UTF8.GetBytes(newKey) });
            _userRepository.Update(user).Complete();
        }
    }
}