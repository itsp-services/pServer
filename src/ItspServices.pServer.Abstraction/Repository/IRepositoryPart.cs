using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IRepositoryPart<T> where T : class
    {
        Task<T> GetByIdAsync(long id);
        Task<IEnumerable<T>> GetAllAsync();
        Task Add();
        Task Remove();
    }
}
