using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Authorization;
using ItspServices.pServer.Authorization.Authorizer;
using ItspServices.pServer.Authorization.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

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
                Id = 0,
                OwnerId = user.Id
            };

            IAuthorizer userAuthorizer = new UserDataOwnerCheck(new UserDataAuthorizer(user, data));

            Assert.IsTrue(userAuthorizer.Authorize());
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
                Id = 0,
                OwnerId = 1
            };

            IAuthorizer userAuthorizer = new UserDataOwnerCheck(new UserDataAuthorizer(user, data));

            Assert.IsFalse(userAuthorizer.Authorize());
        }
    }
}
