using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Persistence.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        UserRepository WriteUserRepository { get; set; }

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
            sampleUser1.PublicKeys.Add(new Key() { Id = 0, KeyData = Encoding.UTF8.GetBytes("cgD") });
            sampleUser1.PublicKeys.Add(new Key() { Id = 1, KeyData = Encoding.UTF8.GetBytes("lHP") });
            sampleUser1.PublicKeys.Add(new Key() { Id = 2, KeyData = Encoding.UTF8.GetBytes("pPV") });

            User sampleUser2 = new User()
            {
                Id = 1,
                UserName = "Foo",
                NormalizedUserName = "FOO",
            };
            sampleUser2.PasswordHash = "AQAAAAEAACcQAAAAEMLzIRUZnL3I6Pf5HnJuV";
            sampleUser2.PublicKeys.Add(new Key() { Id = 0, KeyData = Encoding.UTF8.GetBytes("cgDzAG4AaQBjAGEALAAgAEMASQBG") });
            sampleUser2.PublicKeys.Add(new Key() { Id = 1, KeyData = Encoding.UTF8.GetBytes("lHPrzg5XPAOBOp0KoVdDaaxXbXmQ"), Flag = Key.KeyFlag.OBSOLET });
            sampleUser2.PublicKeys.Add(new Key() { Id = 2, KeyData = Encoding.UTF8.GetBytes("pPVWQxaZLPSkVrQ0uGE3ycJYgBug"), Flag = Key.KeyFlag.OBSOLET });

            _readonlyUserData.Add(sampleUser1);
            _readonlyUserData.Add(sampleUser2);
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ReadUserRepository = new UserRepository(Path.GetFullPath(Path.Combine("Data", "UserRepository", "ReadUserData.xml")));
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
            WriteUserRepository = new UserRepository(Path.Combine("Data", "UserRepository", "AddUserData.xml"));

            User newUser = new User()
            {
                UserName = "FooBar",
                NormalizedUserName = "FOOBAR",
                PasswordHash = "AQAAAAEAACcQAAAAEMLzIRUZnL3I6Pf5HnJuV"
            };
            newUser.PublicKeys.Add(new Key() { Id = 0, KeyData = Encoding.UTF8.GetBytes("cgDzAG4AaQBjAGEALAAgAEMASQBG") });
            newUser.PublicKeys.Add(new Key() { Id = 1, KeyData = Encoding.UTF8.GetBytes("lHPrzg5XPAOBOp0KoVdDaaxXbXmQ") });
            newUser.PublicKeys.Add(new Key() { Id = 2, KeyData = Encoding.UTF8.GetBytes("pPVWQxaZLPSkVrQ0uGE3ycJYgBug") });

            WriteUserRepository.Add(newUser).Complete();
            User userFromFile = WriteUserRepository.GetUserByNormalizedName("FOOBAR");


            Assert.AreNotEqual(null, userFromFile);
            AreEquivalentUser(newUser, userFromFile);
        }

        [TestMethod]
        public void AddUserToEmptyRespository_ShouldSucceed()
        {
            WriteUserRepository = new UserRepository(Path.Combine("Data", "UserRepository", "EmptyAddUserData.xml"));

            User newUser = new User()
            {
                UserName = "FooBar",
                NormalizedUserName = "FOOBAR",
                PasswordHash = "AQAAAAEAACcQAAAAEMLzIRUZnL3I6Pf5HnJuV"
            };
            newUser.PublicKeys.Add(new Key() { Id = 0, KeyData = Encoding.UTF8.GetBytes("cgDzAG4AaQBjAGEALAAgAEMASQBG") });
            newUser.PublicKeys.Add(new Key() { Id = 1, KeyData = Encoding.UTF8.GetBytes("lHPrzg5XPAOBOp0KoVdDaaxXbXmQ") });
            newUser.PublicKeys.Add(new Key() { Id = 2, KeyData = Encoding.UTF8.GetBytes("pPVWQxaZLPSkVrQ0uGE3ycJYgBug") });

            WriteUserRepository.Add(newUser).Complete();
            User userFromFile = WriteUserRepository.GetUserByNormalizedName("FOOBAR");


            Assert.AreNotEqual(null, userFromFile);
            AreEquivalentUser(newUser, userFromFile);
        }

        [TestMethod]
        public void RemoveUser()
        {
            WriteUserRepository = new UserRepository(Path.Combine("Data", "UserRepository", "RemoveUserData.xml"));

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
            WriteUserRepository = new UserRepository(Path.Combine("Data", "UserRepository", "UpdateUserData.xml"));

            User userToUpdate = WriteUserRepository.GetById(_readonlyUserData[1].Id);
            Assert.AreNotEqual(null, userToUpdate);
            AreEquivalentUser(_readonlyUserData[1], userToUpdate);

            userToUpdate.UserName = "BarFoo";
            userToUpdate.NormalizedUserName = userToUpdate.UserName.ToUpper();
            userToUpdate.PublicKeys.Add(new Key() { Id = 3, KeyData = Encoding.UTF8.GetBytes("lHPrzg5XPAOBOp0KoVdDaaxXbXmQ") });

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
            Assert.AreEqual(expected.Role, actual.Role);

            // Collection of keys has to be compared individually because the default implementation
            // of Equals compares references
            for (int index = 0; index < expected.PublicKeys.Count; index++)
            {
                CollectionAssert.AreEqual(expected.PublicKeys[index].KeyData, actual.PublicKeys[index].KeyData);
                Assert.AreEqual(expected.PublicKeys[index].Id, actual.PublicKeys[index].Id);
                Assert.AreEqual(expected.PublicKeys[index].Flag, actual.PublicKeys[index].Flag);
            }
        }

    }
}