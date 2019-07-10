using ItspServices.pServer.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IRepository
    {
        IRepositoryPart<User> UserPart { get; }
    }
}
