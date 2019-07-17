using System;

namespace ItspServices.pServer.Abstraction.Units
{
    public interface IUnitOfWork<T> : IDisposable
    {
        T Entity { get; }

        bool Complete();
    }
}
