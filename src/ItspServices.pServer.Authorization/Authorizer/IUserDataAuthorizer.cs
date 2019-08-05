using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Authorization.Authorizer
{
    public interface IUserDataAuthorizer : IAuthorizer
    {
        User User { get; set; }
        ProtectedData Data { get; set; }
    }
}
