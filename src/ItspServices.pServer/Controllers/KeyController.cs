using System;
using System.Text;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;
using ItspServices.pServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItspServices.pServer.Controllers
{
    [Route("/api/[controller]")]
    [Authorize]
    [ApiController]
    public class KeyController : ControllerBase
    {
        private readonly IRepositoryManager _repository;

        public KeyController(IRepositoryManager repository)
        {
            _repository = repository;
        }

        [HttpPut("edit/{id:int}")]
        public IActionResult EditKey([FromBody]PublicKeyModel model)
        {
            User sessionUser = _repository.UserRepository.GetUserByNormalizedName(User.Identity.Name);
            using (IUpdateUnitOfWork<User, int> uow = _repository.UserRepository.Update(sessionUser.Id))
            {
                uow.Entity.PublicKeys.Find((k) => k.Id == model.KeyNumber).KeyData = Encoding.Default.GetBytes(model.KeyData);
                if (model.Active)
                    uow.Entity.PublicKeys.Find((k) => k.Id == model.KeyNumber).Flag = Key.KeyFlag.ACTIVE;
                else
                    uow.Entity.PublicKeys.Find((k) => k.Id == model.KeyNumber).Flag = Key.KeyFlag.OBSOLET;

                uow.Complete();
            }

            return Ok();
        }

        [HttpPost("add")]
        public IActionResult AddNewKey([FromBody]PublicKeyModel model)
        {
            Console.Write("Model: " + model.KeyData);
            
            if (model.KeyData == null)
                return Ok();

            User sessionUser = _repository.UserRepository.GetUserByNormalizedName(User.Identity.Name.ToUpper());

            using (IUpdateUnitOfWork<User, int> uow = _repository.UserRepository.Update(sessionUser.Id))
            {
                uow.Entity.PublicKeys.Add(new Key() { KeyData = Encoding.UTF8.GetBytes(model.KeyData) });
                uow.Complete();
            }

            return Ok();
        }

    }
}