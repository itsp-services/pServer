using ItspServices.pServer.Authorization.Authorizer;
using System;
using System.Collections.Generic;
using System.Text;

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
