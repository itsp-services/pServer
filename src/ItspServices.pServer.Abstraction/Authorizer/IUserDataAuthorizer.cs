using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Abstraction.Authorizer
{
    public interface IUserDataAuthorizer : IAuthorizer
    {
        User User { get; set; }
        ProtectedData Data { get; set; }
    }
}
