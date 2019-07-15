
using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Abstraction.Units
{
    public interface IUserUnit : IUnitOfWork
    {
        User User { get; }
    }
}
