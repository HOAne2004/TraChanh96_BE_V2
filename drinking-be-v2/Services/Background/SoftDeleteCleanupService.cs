using drinking_be.Data; // Cần để lấy Assembly chứa Model
using drinking_be.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace drinking_be.Services.Background
{
    public class SoftDeleteCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public SoftDeleteCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupAllSoftDeletedEntitiesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Cleanup Error]: {ex.Message}");
                }

                // Chạy mỗi 24h
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task CleanupAllSoftDeletedEntitiesAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var thresholdDate = DateTime.UtcNow.AddDays(-30); // Ngưỡng 30 ngày

                // --- CÁCH 2: TỰ ĐỘNG HÓA HOÀN TOÀN (Reflection) ---

                // 1. Lấy tất cả các class trong Project có implement ISoftDelete
                var softDeleteTypes = typeof(ISoftDelete).Assembly.GetTypes()
                    .Where(t => typeof(ISoftDelete).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

                // 2. Lấy method Generic "DeleteExpiredAsync" bên dưới
                var methodInfo = typeof(SoftDeleteCleanupService)
                    .GetMethod("DeleteExpiredAsync", BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var type in softDeleteTypes)
                {
                    // 3. Tạo version Generic của hàm cho Type hiện tại (ví dụ: DeleteExpiredAsync<News>)
                    var genericMethod = methodInfo?.MakeGenericMethod(type);

                    // 4. Gọi hàm
                    if (genericMethod != null)
                    {
                        var task = (Task)genericMethod.Invoke(this, new object[] { unitOfWork, thresholdDate })!;
                        await task;
                    }
                }

                await unitOfWork.SaveChangesAsync();
            }
        }

        // --- HÀM GENERIC XỬ LÝ CHUNG ---
        // Hàm này sẽ được gọi tự động qua Reflection ở trên
        private async Task DeleteExpiredAsync<T>(IUnitOfWork unitOfWork, DateTime threshold) where T : class, ISoftDelete
        {
            var repo = unitOfWork.Repository<T>();

            // Lọc các bản ghi có DeletedAt quá hạn
            // (Không cần check Status, vì cứ DeletedAt != null nghĩa là đã xóa mềm rồi)
            var expiredItems = await repo.GetAllAsync(
                x => x.DeletedAt != null && x.DeletedAt <= threshold
            );

            if (expiredItems.Any())
            {
                // Gọi hàm DeleteRange (Giả sử bạn đã thêm vào GenericRepo như hướng dẫn trước)
                // Nếu chưa có DeleteRange, dùng vòng lặp repo.Delete(item)
                repo.DeleteRange(expiredItems);

                Console.WriteLine($"[Cleanup Auto] Đã xóa vĩnh viễn {expiredItems.Count()} bản ghi của bảng {typeof(T).Name}");
            }
        }
    }
}