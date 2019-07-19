using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;

namespace ItspServices.pServer.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ProtectedDataController : Controller
    {
        private readonly IRepositoryManager _repository;

        public ProtectedDataController(IRepositoryManager repository)
        {
            _repository = repository;
        }

        [Route("{id:int}"), HttpGet]
        public Task<ProtectedData> GetProtectedData(int id)
        {
            return Task.FromResult(_repository.ProtectedDataRepository.GetById(id));
        }
    }
}