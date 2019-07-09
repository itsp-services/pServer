using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Test.Mock.Implementation;

namespace ItspServices.pServer.Test
{
    [TestClass]
    public class UserDbTest
    {
        [TestMethod]
        public void GetUserJsonTest()
        {
            JsonUserDbHandler userDbHandler = new JsonUserDbHandler();
            User user = userDbHandler.GetUserById(1);
            Assert.AreEqual("Sebastian Muster", user.Name);
            Assert.AreEqual(1, user.Id);
        }

        [TestMethod]
        public void RegisterUserJsonTest()
        {
            JsonUserDbHandler userDbHandler = new JsonUserDbHandler();
            User user = new User
            {
                Id = 2,
                Name = "John Doe"
            };
            byte[] publicKey1 = { 255, 123, 12, 243, 213, 51, 5, 2, 15, 12, 3, 6, 48, 89 };
            byte[] publicKey2 = { 255, 123, 12, 243, 213, 51, 5, 2, 15, 13, 3, 6, 48, 89 };
            user.PublicKeys.Add(publicKey1);
            user.PublicKeys.Add(publicKey2);
            userDbHandler.RegisterUser(user);
        }
    }
}
