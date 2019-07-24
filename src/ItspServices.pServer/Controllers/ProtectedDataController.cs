using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using Microsoft.AspNetCore.Authorization;

namespace ItspServices.pServer.Controllers
{
    [Route("/api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProtectedDataController : Controller
    {
        private readonly IRepositoryManager _repository;

        public ProtectedDataController(IRepositoryManager repository)
        {
            _repository = repository;
        }

        [HttpGet("folder/{id:int?}")]
        public Task<Folder> GetFolderById(int? id)
        {
            return Task.FromResult(_repository.ProtectedDataRepository.GetFolderById(id));
        }
    }
}