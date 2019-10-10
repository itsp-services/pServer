using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction;
using ItspServices.pServer.Models;
using ItspServices.pServer.Abstraction.Authorizer;
using ItspServices.pServer.Authorization;
using ItspServices.pServer.Abstraction.Units;

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
        public Task<FolderModel> GetFolderById(int? id)
        {
            Folder folder = _repository.ProtectedDataRepository.GetFolderById(id);
            return Task.FromResult(
                new FolderModel {
                    ParentId = folder.ParentId,
                    Name = folder.Name,
                    ProtectedDataIds = folder.DataIds,
                    SubfolderIds = folder.SubfolderIds
                });
        }

        // Requires read permission
        [HttpGet("data/{id:int}")]
        public IActionResult GetDataById(int id)
        {
            ProtectedData data = _repository.ProtectedDataRepository.GetById(id);
            if (data == null)
                return NotFound();
            User user = GetSessionUser();
            IAuthorizer authorizer = new UserDataAuthorizerBuilder(user, data)
                                            .AddRequiredPermission(Permission.READ)
                                            .Build();

            if (!authorizer.Authorize())
                return StatusCode(403);

            DataModel dataModel = new DataModel()
            {
                Name = data.Name,
                Data = data.Data
            };

            UserRegisterEntry entry = data.Users.RegisterEntries.Find(x => x.User.Id == user.Id);
            dataModel.KeyPairs = from symmetricKey in entry.EncryptedKeys
                                 join publicKey in user.PublicKeys
                                 on symmetricKey.MatchingPublicKeyId equals publicKey.Id
                                 select new KeyPairModel() {
                                     PublicKey = publicKey.KeyData, SymmetricKey = symmetricKey.KeyData
                                 };

            return Ok(dataModel);
        }

        [HttpPost("data/{folderId:int?}")]
        public IActionResult AddData([FromBody]DataModel model, int? folderId = null)
        {
            User user = GetSessionUser();
            ProtectedData newData = DataFromModel(model);
            newData.OwnerId = user.Id;

            newData.Users.RegisterEntries.Add(new UserRegisterEntry()
            {
                User = user,
                Permission = Permission.WRITE,
                EncryptedKeys = new List<SymmetricKey>(new[] {
                    new SymmetricKey() {
                        KeyData = model.KeyPairs.SingleOrDefault()?.SymmetricKey,
                        MatchingPublicKeyId = (from key in user.PublicKeys
                                               where key.KeyData.SequenceEqual(model.KeyPairs.SingleOrDefault()?.PublicKey)
                                               select key.Id).SingleOrDefault()
                    }
                })
            });

            _repository.ProtectedDataRepository.AddToFolder(newData,
                _repository.ProtectedDataRepository.GetFolderById(folderId)).Complete();

            return Created($"/api/protecteddata/data/{newData.Id}", null);
        }

        [HttpPut("data/{id:int}")]
        public IActionResult UpdateData([FromBody]DataModel model, int id)
        {
            using (IUpdateUnitOfWork<ProtectedData, int> unitOfWork = _repository.ProtectedDataRepository.Update(id))
            {
                if (unitOfWork == null)
                    return NotFound();

                User user = GetSessionUser();
                IAuthorizer authorizer = new UserDataAuthorizerBuilder(user, unitOfWork.Entity)
                                                .AddIsOwnerCheck()
                                                .AddRequiredPermission(Permission.WRITE)
                                                .Build();

                if (!authorizer.Authorize())
                    return StatusCode(403);

                unitOfWork.Entity.Name = model.Name;
                unitOfWork.Entity.Data = model.Data;

                unitOfWork.Complete();

                return Ok();
            }
        }

        [HttpDelete("data/{id:int}")]
        public IActionResult RemoveData(int id)
        {
            using (IRemoveUnitOfWork<ProtectedData, int> unitOfWork = _repository.ProtectedDataRepository.Remove(id))
            {
                User user = GetSessionUser();
                IAuthorizer authorizer = new UserDataAuthorizerBuilder(user, unitOfWork.Entity)
                                                .AddIsOwnerCheck()
                                                .AddRequiredPermission(Permission.WRITE)
                                                .Build();

                if (!authorizer.Authorize())
                    return StatusCode(403);

                unitOfWork.Complete();
                return Ok();

            }
        }

        private User GetSessionUser()
            => _repository.UserRepository.GetUserByNormalizedName(User.Identity.Name.ToUpper());

        private static ProtectedData DataFromModel(DataModel model)
        {
            return new ProtectedData()
            {
                Name = model.Name,
                Data = model.Data
            };
        }
    }
}