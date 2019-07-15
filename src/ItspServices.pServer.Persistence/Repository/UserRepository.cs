using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Persistence.Repository
{
    public class UserRepository : IUserRepository
    {
        public IUnitOfWork Add()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(string id)
        {
            throw new NotImplementedException();
        }

        public User GetUserByName(string name)
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork Remove()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork Update()
        {
            throw new NotImplementedException();
        }
    }
}
