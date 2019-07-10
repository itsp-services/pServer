using System;
using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Repository;

namespace ItspServices.pServer.Repository
{
    public class RepositoryPart<T> : IRepositoryPart<T> where T : class
    {
        public RepositoryPart()
        {
        }

        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public T GetById(long id)
        {
            throw new NotImplementedException();

        }

        public void Remove(T entity)
        {
            throw new NotImplementedException();

        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();

        }
    }
}
