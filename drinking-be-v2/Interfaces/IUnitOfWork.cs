namespace drinking_be.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Có thể khai báo các Repository cụ thể ở đây nếu muốn pattern tập trung
        // IProductRepository Product { get; } 

        Task<int> SaveChangesAsync();
        IGenericRepository<T> Repository<T>() where T : class;
    }
}