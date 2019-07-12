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
        public IActionResult Login([FromBody]LoginModel loginModel, string returnUrl = null)
        {
            //Signin

            loginModel.Password = null;
            ViewData["ReturnUrl"] = returnUrl;

            return View(loginModel);
        }

    }
}