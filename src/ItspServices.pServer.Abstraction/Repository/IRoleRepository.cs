using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IRoleRepository : IRepository<Role>
    {
        Role GetRoleByName(string name);
    }
}
