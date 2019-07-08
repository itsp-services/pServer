using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ItspServices.pServer.Controllers
{
    public class HelloWorldController : Controller
    {
        public string Index()
        {
            return "Index action...";
        }
        
        public IActionResult Welcome(int numTimes, string name)
        {
            ViewData["UserName"] = name;
            ViewData["NumTimes"] = numTimes;

            return View();
        }

        public IEnumerable<object> Values()
        {
            return new[] {
                new { Name = "Test", Value = 3 },
                new { Name = "Test2", Value = 4 },
                new { Name = "Test3", Value = 5 }
            };
        }
    }
}