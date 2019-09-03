using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IAddUnitOfWork<T> Add();
        IUnitOfWork<T> Remove(T entity);
        IUnitOfWork<T> Update(T entity);
    }
}
