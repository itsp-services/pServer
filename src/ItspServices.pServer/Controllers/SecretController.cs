using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ItspServices.pServer.Controllers
{
    public class SecretController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}