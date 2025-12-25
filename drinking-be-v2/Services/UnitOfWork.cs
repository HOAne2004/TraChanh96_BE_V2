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
        private Hashtable _repositories = new();

        public UnitOfWork(DBDrinkContext context)
        {
            _context = context;
        }

        // --- TRIỂN KHAI CÁC REPOSITORY CỤ THỂ ---
        // Sử dụng cú pháp => Repository<T>() để tận dụng cơ chế caching bên dưới
        public IGenericRepository<Store> Stores => Repository<Store>();
        public IGenericRepository<Address> Addresses => Repository<Address>();
        public IGenericRepository<Order> Orders => Repository<Order>();
        public IGenericRepository<ShopTable> ShopTables => Repository<ShopTable>();
        public IGenericRepository<Product> Products => Repository<Product>();
        public IGenericRepository<ProductSize> ProductSizes => Repository<ProductSize>();
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
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