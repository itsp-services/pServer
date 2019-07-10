using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Test
{
    [TestClass]
    public class UserRepositoryTest
    {
        private UserRepository _userRepository;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = new UserRepository(new Data.UserDataContext());
        }

        [TestMethod]
        public void AddUserTest()
        {
            User sampleUser = new User()
            {
                Id = 0,
                Name = "Max Muster"
            };
            sampleUser.PublicKeys.Add(Encoding.ASCII.GetBytes("CCsGAQUFBwICMIIBFB6CARAAQQB1AHQAbwByAGkAZABhAGQAIABkAGUAIABDAGUA"));
            sampleUser.PublicKeys.Add(Encoding.ASCII.GetBytes("lHPrzg5XPAOBOp0KoVdDaaxXbXmQeOW1tDvYvEyNKKGno6e6Ak4l0Squ7a4DIrhr"));

            _userRepository.Add(sampleUser);

        }
    }
}
