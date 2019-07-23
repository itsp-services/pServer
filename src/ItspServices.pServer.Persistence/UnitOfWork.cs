using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;
using System;
using System.Collections.Generic;

namespace ItspServices.pServer.Persistence
{
    class UnitOfWork : IUnitOfWork<User>
    {
        private string _filePath;

        internal TransactionRecord<User> TransactionRecord { get; }

        public UnitOfWork(string filePath)
        {
            _filePath = filePath;
            TransactionRecord = new TransactionRecord<User>();
        }

        public void Complete()
        {
            CommitAdd();
            TransactionRecord.Clear();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }
        private void CommitUpdate()
        {
            throw new NotImplementedException();
        }

        private void CommitRemove()
        {
            throw new NotImplementedException();
        }

        private void CommitAdd()
        {
            IEnumerable<User> stagedUsers = TransactionRecord.StagedEntitiesAsEnumerable(TransactionActions.ADD);
            if (stagedUsers == null)
                return;

            foreach(User user in stagedUsers)
            { 
                
            }
        }
    }
}
