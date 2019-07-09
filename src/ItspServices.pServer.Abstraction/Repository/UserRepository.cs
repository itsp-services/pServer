using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Abstraction.Repository
{
    public class UserRepository
    {

        private IDataService _DataService { get; set; }

        public User GetUserById(long id)
        {
            return _DataService.GetById<User>(id);
        }
    }
}
