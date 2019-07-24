using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Persistence.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            sampleUser1.PublicKeys.Add(new Key("cgD"));
            sampleUser1.PublicKeys.Add(new Key("lHP"));
            sampleUser1.PublicKeys.Add(new Key("pPV"));

            User sampleUser2 = new User()
            {
                Id = 1,
                UserName = "Foo",
                NormalizedUserName = "FOO",
            };
            sampleUser2.PasswordHash = "AQAAAAEAACcQAAAAEMLzIRUZnL3I6Pf5HnJuV";
            sampleUser2.PublicKeys.Add(new Key("cgDzAG4AaQBjAGEALAAgAEMASQBG"));
            sampleUser2.PublicKeys.Add(new Key("lHPrzg5XPAOBOp0KoVdDaaxXbXmQ"));
            sampleUser2.PublicKeys.Add(new Key("pPVWQxaZLPSkVrQ0uGE3ycJYgBug"));

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

        [TestMethod]
        public void AddUser()
        {
            User newUser = new User()
            {
                UserName = "FooBar",
                NormalizedUserName = "FOOBAR",
                PasswordHash = "AQAAAAEAACcQAAAAEMLzIRUZnL3I6Pf5HnJuV"
            };
            newUser.PublicKeys.Add(new Key("cgDzAG4AaQBjAGEALAAgAEMASQBG"));
            newUser.PublicKeys.Add(new Key("lHPrzg5XPAOBOp0KoVdDaaxXbXmQ"));
            newUser.PublicKeys.Add(new Key("pPVWQxaZLPSkVrQ0uGE3ycJYgBug"));

            WriteUserRepository.Add(newUser).Complete();
            User userFromFile = WriteUserRepository.GetUserByNormalizedName("FOOBAR");

            Assert.AreNotEqual(null, userFromFile);
            AreEquivalentUser(newUser, userFromFile);
        }

        [TestMethod]
        public void RemoveUser()
        {
            User user = WriteUserRepository.GetById(_readonlyUserData[0].Id);
            Assert.AreNotEqual(null, user);
            AreEquivalentUser(_readonlyUserData[0], user);

            WriteUserRepository.Remove(_readonlyUserData[0]).Complete();

            user = WriteUserRepository.GetById(_readonlyUserData[0].Id);
            Assert.AreEqual(null, user);
        }

        [TestMethod]
        public void UpdateUser()
        {
            User userToUpdate = WriteUserRepository.GetById(_readonlyUserData[1].Id);
            Assert.AreNotEqual(null, userToUpdate);
            AreEquivalentUser(_readonlyUserData[1], userToUpdate);

            userToUpdate.UserName = "BarFoo";
            userToUpdate.NormalizedUserName = userToUpdate.UserName.ToUpper();
            userToUpdate.PublicKeys.Add(new Key("lHPrzg5XPAOBOp0KoVdDaaxXbXmQ"));

            WriteUserRepository.Update(userToUpdate).Complete();

            User userFromFile = WriteUserRepository.GetById(_readonlyUserData[1].Id);
            Assert.AreNotEqual(null, userFromFile);
            AreEquivalentUser(userToUpdate, userFromFile);
        }

        private static void AreEquivalentUser(User expected, User actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.UserName, actual.UserName);
            Assert.AreEqual(expected.NormalizedUserName, actual.NormalizedUserName);
            Assert.AreEqual(expected.PasswordHash, actual.PasswordHash);

            // Collection of keys has to be compared individually because the default implementation
            // of Equals compares references
            for (int index = 0; index < expected.PublicKeys.Count; index++)
            {
                CollectionAssert.AreEqual(expected.PublicKeys[index].GetKeyAsBytes(), actual.PublicKeys[index].GetKeyAsBytes());
            }
        }

    }
}