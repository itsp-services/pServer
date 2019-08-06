using ItspServices.pServer.Abstraction.Authorizer;

namespace ItspServices.pServer.Authorization.Checks
{
    public class UserDataOwnerCheck : UserDataAuthorizerDecorator
    {
        public UserDataOwnerCheck(IUserDataAuthorizer authorizer)
            : base(authorizer)
        {
        }

        public override bool Authorize()
        {
            bool isOwner = User.Id == Data.OwnerId;
            return isOwner || _userDataAuthorizer.Authorize();
        }
    }
}
