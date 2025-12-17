//using drinking_be.Data;
using drinking_be.Interfaces;
using drinking_be.Models;
using drinking_be.Repositories;
using System.Collections;

namespace drinking_be.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBDrinkContext _context;
        private Hashtable _repositories;

        public UnitOfWork(DBDrinkContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<T>)_repositories[type]!;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}