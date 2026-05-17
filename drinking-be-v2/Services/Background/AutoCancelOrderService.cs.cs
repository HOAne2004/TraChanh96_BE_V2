using drinking_be.Enums;
using drinking_be.Hubs;
using drinking_be.Interfaces;
using drinking_be.Models;
using Microsoft.AspNetCore.SignalR;
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
            while (!stoppingToken.IsCancellationRequested)
            {
                try { await ProcessExpiredOrders(); }
                catch (Exception ex) { _logger.LogError(ex, "Lỗi AutoCancelOrderService"); }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessExpiredOrders()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DBDrinkContext>();
            var settingService = scope.ServiceProvider.GetRequiredService<ISettingService>();

            // 🟢 Lấy IHubContext để bắn Realtime (Bắt buộc phải Inject ở tầng này)
            var hubContext = scope.ServiceProvider.GetService<IHubContext<NotificationHub>>();

            // 1. TÁCH RÕ 2 MỐC THỜI GIAN
            int paymentTimeout = await settingService.GetIntValueAsync("OrderAutoCancelMinutes", 5);
            int confirmTimeout = await settingService.GetIntValueAsync("StoreConfirmTimeoutMinutes", 30); // 30 phút quán quên confirm

            var paymentThreshold = DateTime.UtcNow.AddMinutes(-paymentTimeout);
            var confirmThreshold = DateTime.UtcNow.AddMinutes(-confirmTimeout);

            // 2A. Quét đơn chưa thanh toán online (Áp dụng Payment Threshold)
            var unpaidOrders = await context.Orders
                .Where(o => o.Status == OrderStatusEnum.PendingPayment && o.IsPaid == false && o.CreatedAt < paymentThreshold)
                .ToListAsync();

            // 2B. Quét đơn quán BỎ QUÊN (Áp dụng Confirm Threshold) - Bất kể COD hay Đã trả tiền
            var forgottenOrders = await context.Orders
                .Where(o => o.Status == OrderStatusEnum.New && o.CreatedAt < confirmThreshold)
                .ToListAsync();

            var allOrdersToCancel = unpaidOrders.Concat(forgottenOrders).ToList();

            if (allOrdersToCancel.Any())
            {
                foreach (var order in allOrdersToCancel)
                {
                    order.Status = OrderStatusEnum.Cancelled;
                    order.UpdatedAt = DateTime.UtcNow;

                    // Phân loại lý do Hủy và Xử lý Hoàn tiền
                    if (order.IsPaid)
                    {
                        order.CancelReason = OrderCancelReasonEnum.StoreOverloaded;
                        order.CancelNote = "Cửa hàng quá tải nên không xác nhận đơn. Tiền sẽ được hoàn lại.";
                        // [Nâng cao] Chuyển trạng thái OrderPayment sang RefundPending ở đây nếu cần
                    }
                    else
                    {
                        order.CancelReason = OrderCancelReasonEnum.AutoCancel;
                        order.CancelNote = "Hệ thống tự hủy do quá thời gian chờ.";
                    }

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

                // 🟢 BẮN SIGNALR ĐỂ GIAO DIỆN TỰ NHẢY KHÔNG CẦN F5
                if (hubContext != null)
                {
                    foreach (var order in allOrdersToCancel)
                    {
                        // Gửi đi sự kiện OrderStatusChanged
                        await hubContext.Clients.All.SendAsync("OrderStatusChanged", order.Id, OrderStatusEnum.Cancelled);
                    }
                }
            }
        }
    }
}