using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

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

            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            testUser.PasswordHash = passwordHasher.HashPassword(testUser, "Bar");

            testUser.PublicKeys.Add(Encoding.ASCII.GetBytes("AAAABBBB"));

            _users.Add(testUser);
        }

        public IUnitOfWork Add()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(string id)
        {
            throw new NotImplementedException();
        }

        public User GetUserByName(string name)
        {
            return _users.Find(u => u.NormalizedUserName == name);
        }

        public IUnitOfWork Remove()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork Update()
        {
            throw new NotImplementedException();
        }
    }
}
