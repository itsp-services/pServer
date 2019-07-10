using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IRepositoryPart<T> where T : class
    {
        T GetById(long id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
