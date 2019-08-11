using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction.Authorizer
{
    public interface IAuthorizerBuilder
    {
        IAuthorizer Build();
    }
}
