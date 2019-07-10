using System;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;

namespace ItspServices.pServer.Repository
{
    public class UserRepository : RepositoryPart<User>, IUserRepository
    {

        public UserRepository()
        {
        }

        public User GetUserByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}