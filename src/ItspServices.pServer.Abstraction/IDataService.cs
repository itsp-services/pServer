using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction
{
    public interface IDataService
    {
        List<T> GetAll<T>();
        T GetById<T>(long id);
    }
}
