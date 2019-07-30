using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using ItspServices.pServer.Abstraction;
using System.Linq;
using ItspServices.pServer.Models;
using System.Collections.Generic;

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

        // Requires read permission
        [HttpGet("data/{id:int}")]
        public IActionResult GetDataById(int id)
        {
            ProtectedData data = _repository.ProtectedDataRepository.GetById(id);
            if (data == null)
                return NotFound();

            User user = _repository.UserRepository.GetUserByNormalizedName(User.Identity.Name.ToUpper());
            UserRegisterEntry entry = data.Users.RegisterEntries.Find(x => x.User.Id == user.Id);
            if (entry == null && data.OwnerId != user.Id)
                return StatusCode(403);

            DataModel dataModel = new DataModel()
            {
                Name = data.Name,
                Data = data.Data,
            };

            dataModel.KeyPairs = from symmetricKey in entry.EncryptedKeys
                                 join publicKey in user.PublicKeys
                                 on symmetricKey.MatchingPublicKeyId equals publicKey.Id
                                 select new KeyPairModel() {
                                     PublicKey = publicKey.KeyData, SymmetricKey = symmetricKey.KeyData
                                 };

            return Ok(dataModel);
        }
    }
}