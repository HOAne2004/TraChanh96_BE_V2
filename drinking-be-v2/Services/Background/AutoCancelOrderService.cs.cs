using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;

namespace drinking_be.Services.Background
{
    public class AutoCancelOrderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutoCancelOrderService> _logger;

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

                // Chờ 1 phút rồi quét lại một lần
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessExpiredOrders()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBDrinkContext>();
                var settingService = scope.ServiceProvider.GetRequiredService<ISettingService>(); 

                // 1. Lấy cấu hình động từ Database/Cache (Mặc định 5 phút)
                int timeoutMinutes = await settingService.GetIntValueAsync("OrderAutoCancelMinutes", 5);
                var thresholdTime = DateTime.UtcNow.AddMinutes(-timeoutMinutes);

                // 2. Tìm các đơn hàng quá hạn chưa thanh toán hoặc chưa xác nhận
                var expiredOrders = await context.Orders
                    .Where(o => (o.Status == OrderStatusEnum.PendingPayment || o.Status == OrderStatusEnum.New)
                                && o.CreatedAt < thresholdTime
                                && o.IsPaid == false)
                    .ToListAsync();

                if (expiredOrders.Any())
                {
                    _logger.LogInformation($"Tìm thấy {expiredOrders.Count} đơn hàng quá hạn. Tiến hành hủy...");

                    foreach (var order in expiredOrders)
                    {
                        order.Status = OrderStatusEnum.Cancelled;
                        order.CancelReason = OrderCancelReasonEnum.AutoCancel;
                        order.CancelNote = $"Đơn hàng bị hủy do không xác nhận/thanh toán sau {timeoutMinutes} phút.";
                        order.UpdatedAt = DateTime.UtcNow;

                        // 3. Hoàn lại Voucher chuẩn xác như trong OrderService
                        if (order.UserVoucherId.HasValue)
                        {
                            var userVoucher = await context.UserVouchers.FindAsync(order.UserVoucherId);
                            if (userVoucher != null)
                            {
                                userVoucher.Status = UserVoucherStatusEnum.Unused;
                                userVoucher.UsedDate = null;
                                userVoucher.OrderIdUsed = null;

                                var template = await context.VoucherTemplates.FindAsync(userVoucher.VoucherTemplateId);
                                if (template != null && template.UsedCount > 0)
                                {
                                    template.UsedCount--;
                                }
                            }
                        }
                    }

                    await context.SaveChangesAsync();
                    _logger.LogInformation($"Đã hủy thành công {expiredOrders.Count} đơn hàng quá hạn.");
                }
            }
        }
    }
}