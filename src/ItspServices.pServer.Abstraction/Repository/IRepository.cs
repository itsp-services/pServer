using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IRepository<Tentity, Tkey> where Tentity : class
    {
        Tentity GetById(int id);
        IEnumerable<Tentity> GetAll();
        IAddUnitOfWork<Tentity> Add();
        IRemoveUnitOfWork<Tentity, Tkey> Remove(Tkey key);
        IUpdateUnitOfWork<Tentity, Tkey> Update(Tkey key);
    }
}
