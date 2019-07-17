using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IUnitOfWork<T> Add();
        IUnitOfWork<T> Remove();
        IUnitOfWork<T> Update(int id);
    }
}
