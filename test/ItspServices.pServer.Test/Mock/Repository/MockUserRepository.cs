using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;
using ItspServices.pServer.Test.Mock.Units;

namespace ItspServices.pServer.Test.Mock.Repository
{
    class MockUserRepository : IUserRepository
    {
        private List<User> _users;

        public MockUserRepository()
        {
            _users = new List<User>();

            User testUser = new User()
            {
                Id = 0,
                UserName = "Foo",
                NormalizedUserName = "FOO"
            };
            testUser.PasswordHash = new PasswordHasher<User>().HashPassword(testUser, "Bar123456789");
            _users.Add(testUser);
        }

        public IUnitOfWork Add()
        {
            int availableId = 0;
            foreach(User user in _users)
            {
                if (user.Id != availableId)
                    break;
                availableId++;
            }
            MockAddUserUnit unit = new MockAddUserUnit(_users);
            unit.User.Id = availableId;
            return unit;
        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User GetById(int id)
        {
            return _users.Find(x => x.Id == id);
        }

        public User GetUserByName(string name)
        {
            return _users.Find(u => u.NormalizedUserName == name);
        }

        public IUnitOfWork Remove()
        {
            return new MockRemoveUserUnit(_users);
        }

        public IUnitOfWork Update(int id)
        {
            User userToUpdate = _users.Find(x => x.Id == id);
            MockUpdateUserUnit unit = new MockUpdateUserUnit(userToUpdate);
            unit.User.Id = userToUpdate.Id;
            return unit;
        }
    }
}
