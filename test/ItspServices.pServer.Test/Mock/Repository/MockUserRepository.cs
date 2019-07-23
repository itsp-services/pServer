using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;
using ItspServices.pServer.Persistence;
using Microsoft.AspNetCore.Identity;
using UnitRepositoryTest.Unit;

namespace ItspServices.pServer.Test.Mock.Repository
{
    class MockUserRepository : IUserRepository
    {
        private List<User> _context = new List<User>();
        private MockUserUnit _unitOfWork;

        public MockUserRepository()
        {
            User sampleUser = new User()
            {
                Id = 0,
                UserName = "Foo",
                NormalizedUserName = "FOO",
            };
            sampleUser.PasswordHash = new PasswordHasher<User>().HashPassword(sampleUser, "Bar123456789");
            _context.Add(sampleUser);

            _unitOfWork = new MockUserUnit(_context);

        }

        public IUnitOfWork<User> Add(User entity)
        {
            entity.Id = GetFreeId();
            _unitOfWork.Record.Add(entity, TransactionActions.ADD);
            return _unitOfWork;
        }

        public IEnumerable<User> GetAll()
        {
            return _context;
        }

        public User GetById(int id)
        {
            return _context.Find(user => user.Id == id);
        }

        public User GetUserByNormalizedName(string name)
        {
            return _context.Find(user => user.NormalizedUserName == name);
        }

        public IUnitOfWork<User> Remove(User entity)
        {
            _unitOfWork.Record.Add(entity, TransactionActions.REMOVE);
            return _unitOfWork;
        }

        public IUnitOfWork<User> Update(User entity)
        {
            _unitOfWork.Record.Add(entity, TransactionActions.UPDATE);
            return _unitOfWork;
        }

        private int GetFreeId()
        {
            int availableId = 0;
            foreach (User user in _context)
            {
                if (user.Id != availableId)
                    break;
                availableId++;
            }
            return availableId;
        }
    }
}
