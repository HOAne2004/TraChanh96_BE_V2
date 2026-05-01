using AutoMapper;
using drinking_be.Domain.Orders;
using drinking_be.Dtos.NotificationDtos;
using drinking_be.Dtos.OrderPaymentDtos;
using drinking_be.Enums;
using drinking_be.Hubs;
using drinking_be.Interfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static drinking_be.Services.OrderService;
using Microsoft.Extensions.DependencyInjection;
using static Supabase.Postgrest.Constants;

namespace drinking_be.Services
{
    public class OrderPaymentService : IOrderPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IVnPayService _vnPayService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
                                                                    
        public OrderPaymentService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IVnPayService vnPayService,
            IHttpContextAccessor httpContextAccessor, 
            IHubContext<NotificationHub> hubContext, 
            IServiceProvider serviceProvider,
            IEmailService emailService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _vnPayService = vnPayService;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        public async Task<OrderPaymentReadDto> CreateChargeAsync(long orderId, int paymentMethodId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
                ?? throw new AppException("Đơn hàng không tồn tại.");

            if (order.Status == OrderStatusEnum.Completed || order.Status == OrderStatusEnum.Received)
            {
                throw new AppException("Đơn hàng đã hoàn tất, không thể thanh toán lại.");
            }

            var paymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(paymentMethodId)
                ?? throw new AppException("Phương thức thanh toán không hợp lệ.");

            // 1. Lưu DB
            var payment = new OrderPayment
            {
                OrderId = orderId,
                PaymentMethodId = paymentMethodId,
                PaymentMethodName = paymentMethod.Name,
                Amount = order.GrandTotal,
                Type = OrderPaymentTypeEnum.Charge,
                Status = OrderPaymentStatusEnum.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.OrderPayments.AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            // 2. Map ra DTO để trả về
            var resultDto = _mapper.Map<OrderPaymentReadDto>(payment);

            // 3. 🟢 LOGIC SINH LINK VNPAY 
            if (paymentMethod.PaymentType == PaymentTypeEnum.VNPay && _httpContextAccessor.HttpContext != null)
            {
                string vnpayUrl = _vnPayService.CreatePaymentUrl(order, _httpContextAccessor.HttpContext);
                resultDto.PaymentUrl = vnpayUrl;
            }

            return resultDto;
        }

        public async Task<bool> MarkPaymentSuccessAsync(long paymentId, string transactionCode)
        {
            var payment = await _unitOfWork.OrderPayments.GetByIdAsync(paymentId)
                ?? throw new AppException("Giao dịch không tồn tại.");

            if (payment.Status == OrderPaymentStatusEnum.Paid)
                return true;

            if (payment.Status == OrderPaymentStatusEnum.Refunded)
                throw new AppException("Không thể xác nhận thanh toán cho giao dịch hoàn tiền.");

            payment.Status = OrderPaymentStatusEnum.Paid;
            payment.TransactionCode = transactionCode;
            payment.PaymentDate = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.OrderPayments.Update(payment);

            await _unitOfWork.CompleteAsync();

            await RecalculateOrderPaymentStatusAsync(payment.OrderId);

            await _hubContext.Clients.All.SendAsync("PaymentSuccess", payment.OrderId);
            
            var orderService = _serviceProvider.GetRequiredService<IOrderService>();
            var orderDto = await orderService.GetOrderByIdAsync(payment.OrderId);

            // Lấy sẵn thông tin user để dùng cho việc gửi notification và email
            var user = orderDto.UserId.HasValue ? await _unitOfWork.Repository<User>().GetByIdAsync(orderDto.UserId.Value) : null;

            // =========================================================
            // 3. GỬI IN-APP NOTIFICATION (Quả chuông trên Web)
            // =========================================================

            // 3.1 Báo cho Khách hàng
            if (orderDto.UserId.HasValue)
            {
                await _notificationService.CreateAsync(new NotificationCreateDto
                {
                    UserId = orderDto.UserId.Value,
                    Title = "Thanh toán thành công! 🎉",
                    Content = $"Đơn hàng #{orderDto.OrderCode} của bạn đã được thanh toán {payment.Amount:N0}đ. Quán đang chuẩn bị món nhé!",
                    Type = NotificationTypeEnum.Order,
                    ReferenceId = orderDto.OrderCode
                });
            }

            // 3.2 Báo cho toàn bộ Staff/Manager/Admin
            var staffUsers = await _unitOfWork.Repository<User>()
                .GetAllAsync(u => u.RoleId == UserRoleEnum.Admin || u.RoleId == UserRoleEnum.Manager || u.RoleId == UserRoleEnum.Staff);

            foreach (var staff in staffUsers)
            {
                await _notificationService.CreateAsync(new NotificationCreateDto
                {
                    UserId = staff.Id,
                    Title = "💰 Khách đã chuyển khoản!",
                    Content = $"Đơn hàng #{orderDto.OrderCode} vừa nhận được {payment.Amount:N0}đ qua VietQR.",
                    Type = NotificationTypeEnum.Order,
                    ReferenceId = orderDto.OrderCode
                });
            }

            // =========================================================
            // 4. GỬI EMAIL (Hóa đơn điện tử)
            // =========================================================

            // 4.1 Gửi Receipt cho Khách (Tận dụng luôn biến user đã lấy ở trên)
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                _ = _emailService.SendOrderReceiptEmailAsync(user.Email, orderDto);
            }

            // 4.2 Gửi Alert cho Admin
            string adminEmail = "admin.trachanh1996@yopmail.com"; // Thay bằng email chủ quán
            _ = _emailService.SendAdminPaymentAlertEmailAsync(adminEmail, orderDto, payment.Amount);

            return true;
        }
        public async Task<bool> MarkPaymentFailedAsync(long paymentId, string reason)
        {
            var payment = await _unitOfWork.OrderPayments.GetByIdAsync(paymentId)
                ?? throw new AppException("Giao dịch không tồn tại.");

            if (payment.Status != OrderPaymentStatusEnum.Pending)
                return false;

            payment.Status = OrderPaymentStatusEnum.Failed;
            payment.UpdatedAt = DateTime.UtcNow;

            payment.PaymentSignature = reason;

            _unitOfWork.OrderPayments.Update(payment);

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<OrderPaymentReadDto> RefundAsync(long orderId, decimal amount, string reason)
        {
            if (amount <= 0)
                throw new AppException("Số tiền hoàn phải lớn hơn 0.");

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
                ?? throw new AppException("Order không tồn tại.");

            var snapshot = await BuildPaymentSnapshotAsync(orderId);

            if (snapshot.PaidAmount < amount)
                throw new AppException("Số tiền hoàn vượt quá số tiền đã thanh toán.");

            var refundPayment = new OrderPayment
            {
                OrderId = orderId,
                PaymentMethodId = order.PaymentMethodId ?? 0,
                PaymentMethodName = order.PaymentMethodName ?? "Refund",
                Amount = -amount,
                Type = OrderPaymentTypeEnum.Refund,
                Status = OrderPaymentStatusEnum.Paid,
                PaymentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                PaymentSignature = reason
            };

            await _unitOfWork.OrderPayments.AddAsync(refundPayment);

            await RecalculateOrderPaymentStatusAsync(orderId);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderPaymentReadDto>(refundPayment);
        }


        public async Task<bool> RecalculateOrderPaymentStatusAsync(long orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
                ?? throw new AppException("Order không tồn tại.");

            var snapshot = await BuildPaymentSnapshotAsync(orderId);

            if (snapshot.IsFullyPaid(order.GrandTotal))
            {
                if (!order.IsPaid) 
                {
                    order.IsPaid = true;
                    order.PaymentDate = DateTime.UtcNow;

                    if (order.Status == OrderStatusEnum.PendingPayment)
                    {
                        order.Status = OrderStatusEnum.New;
                    }

                    _unitOfWork.Orders.Update(order);
                    await _unitOfWork.CompleteAsync(); 
                }
            }

            return true;
        }

        public async Task<OrderPaymentSnapshot> BuildPaymentSnapshotAsync(long orderId)
        {
            var paidAmount = await _unitOfWork.OrderPayments
                .Find(p => p.OrderId == orderId &&
                           p.Status == OrderPaymentStatusEnum.Paid)
                .SumAsync(p => p.Amount);

            return new OrderPaymentSnapshot
            {
                PaidAmount = paidAmount
            };
        }
        
        public async Task AutoConfirmPaymentAsync(long orderId, int paymentMethodId, string paymentMethodName, decimal amount, string note)
        {
            // 1. Tạo bản ghi thanh toán
            var payment = new OrderPayment
            {
                OrderId = orderId,
                PaymentMethodId = paymentMethodId,
                PaymentMethodName = paymentMethodName,
                Amount = amount,
                TransactionCode = $"AUTO-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                Status = OrderPaymentStatusEnum.Paid,
                Type = OrderPaymentTypeEnum.Charge,
                PaymentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<OrderPayment>().AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            // 2. Cập nhật trạng thái IsPaid của đơn hàng
            await RecalculateOrderPaymentStatusAsync(orderId);

            // =========================================================
            // 3. 🟢 LOGIC BỔ SUNG: GỬI THÔNG BÁO VÀ EMAIL (LAZY LOAD)
            // =========================================================
            try
            {
                // Sử dụng _serviceProvider để lấy IOrderService (Tránh lỗi vòng lặp DI)
                var orderService = _serviceProvider.GetRequiredService<IOrderService>();
                var orderDto = await orderService.GetOrderByIdAsync(orderId);

                var user = orderDto.UserId.HasValue ? await _unitOfWork.Repository<User>().GetByIdAsync(orderDto.UserId.Value) : null;

                // 3.1 Gửi Notification (In-App) cho khách
                if (orderDto.UserId.HasValue)
                {
                    await _notificationService.CreateAsync(new NotificationCreateDto
                    {
                        UserId = orderDto.UserId.Value,
                        Title = "Thanh toán thành công! 🎉",
                        Content = $"Đơn hàng #{orderDto.OrderCode} đã được thanh toán {amount:N0}đ qua {paymentMethodName}.",
                        Type = NotificationTypeEnum.Order,
                        ReferenceId = orderDto.OrderCode
                    });
                }

                // 3.2 Gửi Email Hóa đơn cho khách
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    _ = _emailService.SendOrderReceiptEmailAsync(user.Email, orderDto);
                }

                // 3.3 Gửi Email Alert cho Admin
                string adminEmail = "admin.trachanh1996@yopmail.com"; // Thay bằng email thật
                _ = _emailService.SendAdminPaymentAlertEmailAsync(adminEmail, orderDto, amount);
            }
            catch (Exception)
            {
                // Bọc try-catch để nếu lỗi gửi mail cũng không làm sập luồng chính (không bị throw lỗi 500 ra ngoài)
                // Nếu có ILogger, bạn có thể log lỗi ở đây
            }
        }

        public async Task<decimal> GetTotalRefundedAsync(long orderId)
        {
            return await _unitOfWork.OrderPayments
                .Find(p => p.OrderId == orderId && p.Type == OrderPaymentTypeEnum.Refund && p.Status == OrderPaymentStatusEnum.Paid)
                .SumAsync(p => p.Amount); 
        }

        public async Task<decimal> GetTotalRefundedOverallAsync(int? storeId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _unitOfWork.OrderPayments
                .Find(p => p.Type == OrderPaymentTypeEnum.Refund && p.Status == OrderPaymentStatusEnum.Paid)
                .AsQueryable();

            if (storeId.HasValue)
            {
                query = query.Where(p => p.Order.StoreId == storeId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate <= toDate.Value);
            }

            return await query.SumAsync(p => p.Amount); // Số âm
        }


        public async Task<bool> ProcessSePayWebhookAsync(SePayWebhookDto payload)
        {
            // 1. Tìm đơn hàng dựa trên nội dung chuyển khoản (chứa OrderCode)
            // Cần một logic tìm kiếm thông minh vì nội dung CK có thể có ký tự thừa
            var orderCode = ExtractOrderCodeFromContent(payload.Content!);
            if (string.IsNullOrEmpty(orderCode)) return false;

            var order = await _unitOfWork.Orders.Find(o => o.OrderCode == orderCode).FirstOrDefaultAsync();
            if (order == null) return false;

            // 2. Tìm hoặc tạo OrderPayment tương ứng
            var payment = await _unitOfWork.OrderPayments.Find(p => p.OrderId == order.Id && p.Status == OrderPaymentStatusEnum.Pending).FirstOrDefaultAsync();

            if (payment == null)
            {
                // Nếu không tìm thấy payment pending, có thể khách chuyển thẳng không qua nút "Thanh toán ngay"
                // Tùy nghiệp vụ bạn có muốn tự tạo record mới không
                return false;
            }

            // 3. (Tùy chọn) Kiểm tra số tiền chuyển có khớp không
            // if (payload.transferAmount < payment.Amount) { /* Xử lý thiếu tiền */ }

            // 4. Gọi hàm MarkPaymentSuccessAsync đã có sẵn (hoặc tái sử dụng logic trong đó)
            return await MarkPaymentSuccessAsync(payment.Id, payload.Code!);
        }

        // Helper function để trích xuất mã đơn hàng
        private string ExtractOrderCodeFromContent(string content)
        {
            // Regex bắt chữ ORD, theo sau là cụm số (Group 1), và cụm 4 chữ số đuôi (Group 2)
            // Dấu [- ]? nghĩa là có thể có hoặc không có dấu gạch ngang/khoảng trắng
            var match = System.Text.RegularExpressions.Regex.Match(
                content,
                @"ORD[- ]?(\d+)[- ]?([A-Z0-9]{4})",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match.Success)
            {
                // Lắp ráp lại thành chuẩn ORD-XXXXXX-YYYY để query vào Database
                string numberPart = match.Groups[1].Value;
                string tailPart = match.Groups[2].Value;

                return $"ORD-{numberPart}-{tailPart}".ToUpper();
            }

            return string.Empty;
        }
    }
}
