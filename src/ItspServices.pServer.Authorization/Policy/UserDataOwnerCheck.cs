using ItspServices.pServer.Authorization.Authorizer;

namespace ItspServices.pServer.Authorization.Policy
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
