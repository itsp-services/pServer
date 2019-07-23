using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Persistence.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ItspServices.pServer.RepositoryTest
{
    [TestClass]
    public class UserRepositoryTests
    {

        static UserRepository ReadUserRepository { get; set; }
        static UserRepository WriteUserRepository { get; set; }

        private readonly List<User> _readonlyUserData;

        public UserRepositoryTests()
        {
            _readonlyUserData = new List<User>();

            User sampleUser1 = new User()
            {
                Id = 0,
                UserName = "Bar",
                NormalizedUserName = "BAR",
            };
            sampleUser1.PasswordHash = "AQAAAAEAA";
            sampleUser1.PublicKeys.Add(Encoding.UTF8.GetBytes("cgD"));
            sampleUser1.PublicKeys.Add(Encoding.UTF8.GetBytes("lHP"));
            sampleUser1.PublicKeys.Add(Encoding.UTF8.GetBytes("pPV"));

            User sampleUser2 = new User()
            {
                Id = 1,
                UserName = "Foo",
                NormalizedUserName = "FOO",
            };
            sampleUser2.PasswordHash = "AQAAAAEAACcQAAAAEMLzIRUZnL3I6Pf5HnJuV";
            sampleUser2.PublicKeys.Add(Encoding.UTF8.GetBytes("cgDzAG4AaQBjAGEALAAgAEMASQBG"));
            sampleUser2.PublicKeys.Add(Encoding.UTF8.GetBytes("lHPrzg5XPAOBOp0KoVdDaaxXbXmQ"));
            sampleUser2.PublicKeys.Add(Encoding.UTF8.GetBytes("pPVWQxaZLPSkVrQ0uGE3ycJYgBug"));

            _readonlyUserData.Add(sampleUser1);
            _readonlyUserData.Add(sampleUser2);
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ReadUserRepository = new UserRepository(Path.GetFullPath("Data\\ReadonlyUserData.xml"));
            WriteUserRepository = new UserRepository(Path.GetFullPath("Data\\WriteUserData.xml"));
        }

        [TestMethod]
        public void GetUserById()
        {
            User actualUser = ReadUserRepository.GetById(0);

            Assert.AreNotEqual(null, actualUser);
            AreEquivalentUser(_readonlyUserData[0], actualUser);
        }

        [TestMethod]
        public void GetUserByInvalidId()
        {
            User user = ReadUserRepository.GetById(99);
            Assert.AreEqual(null, user);
        }

        [TestMethod]
        public void GetUserByNormalizedName()
        {
            User actualUser = ReadUserRepository.GetUserByNormalizedName("BAR");

            Assert.AreNotEqual(null, actualUser);
            AreEquivalentUser(_readonlyUserData[0], actualUser);
        }

        [TestMethod]
        public void GetUserByInvalidNormalizedName()
        {
            User user = ReadUserRepository.GetUserByNormalizedName("FooBar");
            Assert.AreEqual(null, user);
        }

        [TestMethod]
        public void GetAllUsers()
        {
            List<User> users = ReadUserRepository.GetAll().ToList();

            Assert.AreNotEqual(null, users);
            for (int i = 0; i < users.Count; i++)
            {
                AreEquivalentUser(_readonlyUserData[i], users[i]);
            }
        }

        public static void AreEquivalentUser(User expected, User actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.UserName, actual.UserName);
            Assert.AreEqual(expected.NormalizedUserName, actual.NormalizedUserName);
            Assert.AreEqual(expected.PasswordHash, actual.PasswordHash);

            // Collection of keys has to be compared individually because the default implementation
            // of Equals compares references
            for (int index = 0; index < expected.PublicKeys.Count; index++)
            {
                CollectionAssert.AreEqual(expected.PublicKeys[index], actual.PublicKeys[index]);
            }
        }

    }
}