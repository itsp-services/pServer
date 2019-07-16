using ItspServices.pServer.Abstraction.Units;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IRepositoryPart<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IUnitOfWork Add();
        IUnitOfWork Remove();
        IUnitOfWork Update(int id);
    }
}
