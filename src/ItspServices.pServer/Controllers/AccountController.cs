using System.Threading.Tasks;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItspServices.pServer.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<User> _signInManager;

        public AccountController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm]LoginModel loginModel, [FromQuery]string returnUrl = null)
        {
            //Signin
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, false, false);

            loginModel.Password = null;
            ViewData["ReturnUrl"] = returnUrl;

            if (result.Succeeded)
            {
                return Redirect(returnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(loginModel);
        }

    }
}