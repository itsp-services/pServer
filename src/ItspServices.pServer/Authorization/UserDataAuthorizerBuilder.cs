using ItspServices.pServer.Abstraction;
using ItspServices.pServer.Abstraction.Authorizer;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Authorization.Checks;

namespace ItspServices.pServer.Authorization
{
    public class UserDataAuthorizerBuilder : IAuthorizerBuilder
    {
        private IUserDataAuthorizer _authorizer;

        public UserDataAuthorizerBuilder(User user, ProtectedData data)
        {
            _authorizer = new UserDataAuthorizer(user, data);
        }

        public UserDataAuthorizerBuilder AddRequiredPermission(Permission permission)
        {
            _authorizer = new UserDataPermissionCheck(_authorizer, permission);
            return this;
        }

        public UserDataAuthorizerBuilder AddIsOwnerCheck()
        {
            _authorizer = new UserDataOwnerCheck(_authorizer);
            return this;
        }

        public IAuthorizer Build()
        {
            return _authorizer;
        }
    }
}
