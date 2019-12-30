using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;

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
            Key key = new Key() { KeyData = Encoding.Default.GetBytes(newKey) };
            using (IUnitOfWork<Key> unitOfWork = _userRepository.AddPublicKey(user, key))
            {
                unitOfWork.Complete();
            }
        }
    }
}