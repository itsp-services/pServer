using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction
{
    public interface IDataServiceContext<T> where T : class
    {
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        T Find(long id);
        List<T> ToList();
    }
}
