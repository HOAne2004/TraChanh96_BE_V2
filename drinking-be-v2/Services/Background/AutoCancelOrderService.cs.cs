using drinking_be.Data;
using drinking_be.Enums;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services.Background
{
    public class AutoCancelOrderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutoCancelOrderService> _logger;

        // Cấu hình thời gian hết hạn (ví dụ 15 phút)
        private const int TIMEOUT_MINUTES = 15;

        public AutoCancelOrderService(IServiceProvider serviceProvider, ILogger<AutoCancelOrderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("⏳ AutoCancelOrderService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessExpiredOrders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi chạy AutoCancelOrderService");
                }

                // Chờ 1 phút rồi chạy lại (tránh spam DB liên tục)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessExpiredOrders()
        {
            // Vì BackgroundService là Singleton, mà DbContext là Scoped
            // Nên phải tạo Scope thủ công
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBDrinkContext>();

                var thresholdTime = DateTime.UtcNow.AddMinutes(-TIMEOUT_MINUTES);

                // Tìm các đơn hàng:
                // 1. Trạng thái là NEW hoặc PENDING_PAYMENT
                // 2. Thời gian tạo < (Hiện tại - 15 phút)
                var expiredOrders = await context.Orders
                    .Where(o => (o.Status == OrderStatusEnum.New || o.Status == OrderStatusEnum.PendingPayment)
                                && o.CreatedAt < thresholdTime)
                    .ToListAsync();

                if (expiredOrders.Any())
                {
                    _logger.LogInformation($"Tìm thấy {expiredOrders.Count} đơn hàng quá hạn. Tiến hành hủy...");

                    foreach (var order in expiredOrders)
                    {
                        order.Status = OrderStatusEnum.Cancelled;
                        order.CancelReason = OrderCancelReasonEnum.AutoCancel;
                        order.CancelNote = $"Đơn hàng bị hủy do không xác nhận/thanh toán sau {TIMEOUT_MINUTES} phút.";
                        order.UpdatedAt = DateTime.UtcNow;

                        // (Tùy chọn) Hoàn lại Voucher nếu cần
                        // Logic hoàn voucher nên tách ra hàm chung trong Service để tái sử dụng
                        // Nhưng ở đây viết nhanh:
                        
                        if (order.UserVoucherId.HasValue) {
                             var voucher = await context.UserVouchers.FindAsync(order.UserVoucherId);
                             if(voucher != null) { 
                                voucher.Status = UserVoucherStatusEnum.Unused; 
                                voucher.OrderIdUsed = null;
                             }
                        }
                        
                    }

                    await context.SaveChangesAsync();
                    _logger.LogInformation("Đã hủy thành công các đơn hàng quá hạn.");
                }
            }
        }
    }
}