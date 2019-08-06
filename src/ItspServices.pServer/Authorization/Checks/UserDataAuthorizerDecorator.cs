using ItspServices.pServer.Abstraction.Authorizer;
using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Authorization.Checks
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

        protected abstract bool Authorize();

        bool IAuthorizer.Authorize()
            => this.Authorize() || _userDataAuthorizer.Authorize();
    }
}
