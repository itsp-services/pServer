using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;

namespace ItspServices.pServer.Controllers
{
    public class ProtectedDataController : Controller
    {
        readonly IRepositoryManager _repository;

        public ProtectedDataController(IRepositoryManager repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public Task<User> GetFolder(int? id)
        {
            return null;
        }

        public Task<ProtectedData> GetProtectedData(int id)
        {
            return null;
        }

        public Task<ProtectedData> GetProtectedData(string fullQualifiedName)
        {
            return null;
        }

    }
}