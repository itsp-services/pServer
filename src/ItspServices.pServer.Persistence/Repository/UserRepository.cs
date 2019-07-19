using System;
using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.Repository
{
    class UserRepository : IUserRepository
    {
        public IUnitOfWork<User> Add(User entity)
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

        public IUnitOfWork<User> Remove(User entity)
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork<User> Update(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
