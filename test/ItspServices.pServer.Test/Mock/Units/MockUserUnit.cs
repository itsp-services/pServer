using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;

namespace UnitRepositoryTest.Unit
{
    internal class MockUserUnit : IUnitOfWork<User>
    {
        private readonly List<User> _context;
        internal TransactionRecord<User> Record { get; }

        public MockUserUnit(List<User> context)
        {
            _context = context;
            Record = new TransactionRecord<User>();
        }

        public void Complete()
        {
            CommitAdd();
            CommitRemove();
            CommitUpdate();
            Record.Clear();
        }

        public void Rollback()
        {
            Record.Clear();
        }

        private void CommitAdd()
        {
            IEnumerable<User> stagedEntities = Record.StagedEntitiesAsEnumerable(TransactionActions.ADD);
            if (stagedEntities == null)
                return;
            foreach (User user in stagedEntities)
            {
                _context.Add(user);
            }
        }

        private void CommitRemove()
        {
            IEnumerable<User> stagedEntities = Record.StagedEntitiesAsEnumerable(TransactionActions.REMOVE);
            if (stagedEntities == null)
                return;
            foreach (User user in stagedEntities)
            {
                _context.Remove(_context.Find(x => x.Id == user.Id));
            }
        }

        private void CommitUpdate()
        {
            IEnumerable<User> stagedEntities = Record.StagedEntitiesAsEnumerable(TransactionActions.UPDATE);
            if (stagedEntities == null)
                return;
            foreach (User user in stagedEntities)
            {
                User userToUpdate = _context.Find(x => x.Id == user.Id);
                userToUpdate = user;
            }
        }

        public void Dispose()
        {
            Rollback();
        }
    }
}
