using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Authorization
{
    public interface IAuthorizer
    {
        bool Authorize();
    }
}
