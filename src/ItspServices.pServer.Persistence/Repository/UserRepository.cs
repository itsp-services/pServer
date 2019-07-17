using System;
using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.Repository
{
    class UserRepository : IUserRepository
    {
        public IUnitOfWork Add()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(int id)
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

        public IUnitOfWork Update(int id)
        {
            throw new NotImplementedException();
        }
    }
}
