using drinking_be.Models;

namespace drinking_be.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Có thể khai báo các Repository cụ thể ở đây nếu muốn pattern tập trung
        IGenericRepository<Store> Stores { get; }
        IGenericRepository<Address> Addresses { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<ShopTable> ShopTables { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<ProductSize> ProductSizes { get; }
        Task<int> SaveChangesAsync();
        Task<int> CompleteAsync();
        IGenericRepository<T> Repository<T>() where T : class;
    }
}