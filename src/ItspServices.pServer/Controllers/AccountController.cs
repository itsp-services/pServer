using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Models;

namespace ItspServices.pServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;

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

        [HttpGet]
        public IActionResult Register(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            return View(new RegisterModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm]RegisterModel registerModel, [FromQuery]string returnUrl = null)
        {
            User user = new User()
            {
                UserName = registerModel.Username,
                NormalizedUserName = registerModel.Username.ToUpper(),
            };

            IdentityResult result = await _signInManager.UserManager.CreateAsync(user, registerModel.Password);
            if(!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Could not create new user");
                return View(registerModel);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult 
                = await _signInManager.PasswordSignInAsync(registerModel.Username, registerModel.Password, false, false);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, "Invalid register attempt.");
            return View(registerModel);
        }
    }
}