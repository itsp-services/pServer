using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Data;

namespace ItspServices.pServer.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {

        public UserRepository(UserDataContext context) : base(context)
        {
        }

        public User GetUserByName(string name)
        {
            foreach (User user in GetAll())
            {
                if (user.Name == name) return user;
            }
            return null;
        }
    }
}
