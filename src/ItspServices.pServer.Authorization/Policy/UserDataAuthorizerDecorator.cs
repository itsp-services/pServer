using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Authorization.Authorizer;

namespace ItspServices.pServer.Authorization.Policy
{
    public abstract class UserDataAuthorizerDecorator : IUserDataAuthorizer
    {
        protected readonly IUserDataAuthorizer _userDataAuthorizer;
        public User User { get => _userDataAuthorizer.User; set => _userDataAuthorizer.User = value; }
        public ProtectedData Data { get => _userDataAuthorizer.Data; set => _userDataAuthorizer.Data = value; }

        protected UserDataAuthorizerDecorator(IUserDataAuthorizer authorizer)
        {
            _userDataAuthorizer = authorizer;
        }

        public abstract bool Authorize();
    }
}
