using System.Linq.Expressions;

namespace drinking_be.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Lấy tất cả (có thể kèm điều kiện lọc và include)
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null // Ví dụ: "Category,Brand"
        );

        // Lấy 1 bản ghi
        Task<T?> GetByIdAsync(object id);

        // Lấy 1 bản ghi kèm điều kiện và include (Quan trọng cho Detail API)
        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> filter,
            string? includeProperties = null
        );
        IQueryable<T> Find(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        // Kiểm tra tồn tại (Tối ưu hơn lấy cả object)
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);

        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);

        IQueryable<T> GetQueryable();
    }
}