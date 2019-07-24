using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return View();
        }
    }
}