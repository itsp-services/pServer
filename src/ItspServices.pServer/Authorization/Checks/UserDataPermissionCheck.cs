using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItspServices.pServer.Abstraction;
using ItspServices.pServer.Abstraction.Authorizer;

namespace ItspServices.pServer.Authorization.Checks
{
    public class UserDataPermissionCheck : UserDataAuthorizerDecorator
    {
        private Permission _requiredPermission;

        public UserDataPermissionCheck(IUserDataAuthorizer authorizer, Permission requiredPermission) 
            : base(authorizer)
        {
            _requiredPermission = requiredPermission;
        }

        protected override bool Authorize()
        {
            Permission? p = Data.Users.RegisterEntries.Find(x => x.User.Id == User.Id)?.Permission;
            return (p == null) ? false : ((int)p >= (int)_requiredPermission);
        }
    }
}
