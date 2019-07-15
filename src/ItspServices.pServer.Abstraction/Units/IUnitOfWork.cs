using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction.Units
{
    public interface IUnitOfWork : IDisposable
    {
        void Complete();
    }
}
