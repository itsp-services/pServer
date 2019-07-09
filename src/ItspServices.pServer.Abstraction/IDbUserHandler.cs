using System;
using System.Collections.Generic;
using System.Text;
using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Abstraction
{
    public interface IDbUserHandler
    {
        User GetUserById(int id);
        void RemoveUserById(int id);
        void RegisterUser(User u);
    }
}
