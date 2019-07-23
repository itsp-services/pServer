using System.Collections.Generic;

namespace ItspServices.pServer.Persistence
{
    public enum TransactionActions
    {
        ADD,
        REMOVE,
        UPDATE
    }

    public class TransactionRecord<T>
    {
        private Dictionary<TransactionActions, List<T>> _record;

        public TransactionRecord()
        {
            _record = new Dictionary<TransactionActions, List<T>>();
        }

        public void Add(T entity, TransactionActions action)
        {
            List<T> stagedEntities;
            if (!_record.TryGetValue(action, out stagedEntities))
            {
                stagedEntities = new List<T>();
                _record.Add(action, stagedEntities);
            }
            stagedEntities.Add(entity);
        }

        public IEnumerable<T> StagedEntitiesAsEnumerable(TransactionActions action)
        {
            List<T> stagedEntitiesList;
            return _record.TryGetValue(action, out stagedEntitiesList) ?
                stagedEntitiesList :
                null;
        }

        public void Clear()
        {
            _record.Clear();
        }
    }
}
