using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Abstraction;
using ItspServices.pServer.Abstraction.Authorizer;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Authorization;
using ItspServices.pServer.Authorization.Checks;

namespace ItspServices.pServer.AuthorizationTest
{
    [TestClass]
    public class AuthorizeTests
    {
        [TestMethod]
        public void AuthorizerUserData_IsOwnerTest()
        {
            User user = new User()
            {
                Id = 0
            };

            ProtectedData data = new ProtectedData()
            {
                OwnerId = user.Id
            };

            IAuthorizer authorizer = new UserDataOwnerCheck(new UserDataAuthorizer(user, data));

            Assert.IsTrue(authorizer.Authorize());
        }

        [TestMethod]
        public void AuthorizerUserData_IsNotOwnerTest()
        {
            User user = new User()
            {
                Id = 0
            };

            ProtectedData data = new ProtectedData()
            {
                OwnerId = 1
            };

            IAuthorizer authorizer = new UserDataOwnerCheck(new UserDataAuthorizer(user, data));

            Assert.IsFalse(authorizer.Authorize());
        }

        [TestMethod]
        public void AuthorizerUserData_UserHasPermissionTest()
        {
            User user = new User()
            {
                Id = 0
            };

            ProtectedData data = new ProtectedData();
            data.Users.RegisterEntries.Add(new UserRegisterEntry()
            {
                Permission = Permission.WRITE,
                User = user
            });

            IAuthorizer authorizer = new UserDataPermissionCheck(new UserDataAuthorizer(user, data), Permission.WRITE);

            Assert.IsTrue(authorizer.Authorize());
        }

        [TestMethod]
        public void AuthorizerUserData_UserHasNoPermissionTest()
        {
            User user = new User()
            {
                Id = 0
            };

            ProtectedData data = new ProtectedData();
            data.Users.RegisterEntries.Add(new UserRegisterEntry()
            {
                Permission = Permission.VIEW,
                User = user
            });

            IAuthorizer authorizer = new UserDataPermissionCheck(new UserDataAuthorizer(user, data), Permission.WRITE);

            Assert.IsFalse(authorizer.Authorize());
        }
    }
}
