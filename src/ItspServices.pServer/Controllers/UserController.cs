using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            User user = _userRepository.GetUserByNormalizedName(User.Identity.Name.ToUpper());
            user.PublicKeys.Add(new Key(newKey));
            _userRepository.Update(user).Complete();
        }
    }
}