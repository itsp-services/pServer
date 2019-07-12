using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction.Adapter
{
    public interface IUserStoreAdapter : IUserStore<User>
    {
        IUserRepository UserRepository { get; }
    }
}
