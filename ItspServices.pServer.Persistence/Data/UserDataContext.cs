using ItspServices.pServer.Abstraction;
using ItspServices.pServer.Abstraction.Models;
using System;
using System.Collections.Generic;

namespace ItspServices.pServer.Data
{
    public class UserDataContext : IDataServiceContext<User>
    {
        public void Add(User entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<User> entities)
        {
            throw new NotImplementedException();
        }

        public User Find(long id)
        {
            throw new NotImplementedException();
        }

        public void Remove(User entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<User> entities)
        {
            throw new NotImplementedException();
        }

        public List<User> ToList()
        {
            throw new NotImplementedException();
        }
    }
}
