using ItspServices.pServer.Abstraction;
using ItspServices.pServer.Abstraction.Repository;
using System.Collections.Generic;

namespace ItspServices.pServer.Repository
{
    public class Repository<T> : IRepositoryPart<T> where T : class
    {
        protected readonly IDataServiceContext<T> _context;

        public Repository(IDataServiceContext<T> context)
        {
            _context = context;

        }

        public void Add(T entity)
        {
            _context.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _context.AddRange(entities);
        }

        public IEnumerable<T> GetAll()
        {
            return _context.ToList();
        }

        public T GetById(long id)
        {
            return _context.Find(id);
        }

        public void Remove(T entity)
        {
            _context.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.RemoveRange(entities);
        }
    }
}
