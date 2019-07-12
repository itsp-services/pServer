using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItspServices.pServer.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<User> _signInManager;

        AccountController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel, string returnUrl = null)
        {
            //Signin

            loginModel.Password = null;
            ViewData["ReturnUrl"] = returnUrl;

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, false, false);
            if(result.Succeeded)
                return Redirect(returnUrl);
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(loginModel);
        }

    }
}