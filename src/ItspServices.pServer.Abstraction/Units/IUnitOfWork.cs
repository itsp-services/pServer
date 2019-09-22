using System;

namespace ItspServices.pServer.Abstraction.Units
{
    public interface IUnitOfWork<T> : IDisposable
    {
        void Complete();
    }
}
