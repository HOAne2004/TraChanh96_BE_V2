using AutoMapper;
using drinking_be.Domain.Orders;
using drinking_be.Dtos.OrderPaymentDtos;
using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Interfaces.OrderInterfaces;
using drinking_be.Models;
using Microsoft.EntityFrameworkCore;
using static drinking_be.Services.OrderService;
using static Supabase.Postgrest.Constants;

namespace drinking_be.Services
{
    public class OrderPaymentService : IOrderPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderPaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderPaymentReadDto> CreateChargeAsync(long orderId, int paymentMethodId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
                ?? throw new AppException("Đơn hàng không tồn tại.");

            if (order.Status == OrderStatusEnum.Completed|| order.Status == OrderStatusEnum.Received)
            {
                throw new AppException("Đơn hàng đã hoàn tất, không thể thanh toán lại.");
            }

            var paymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(paymentMethodId)
                ?? throw new AppException("Phương thức thanh toán không hợp lệ.");

            var payment = new OrderPayment
            {
                OrderId = orderId,
                PaymentMethodId = paymentMethodId,
                PaymentMethodName = paymentMethod.Name, // snapshot
                Amount = order.GrandTotal,               // 🔑 số tiền cần thu
                Type = OrderPaymentTypeEnum.charge,
                Status = OrderPaymentStatusEnum.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.OrderPayments.AddAsync(payment);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderPaymentReadDto>(payment);
        }


        public async Task<bool> MarkPaymentSuccessAsync(long paymentId, string transactionCode)
        {
            var payment = await _unitOfWork.OrderPayments.GetByIdAsync(paymentId)
                ?? throw new AppException("Giao dịch không tồn tại.");

            if (payment.Status == OrderPaymentStatusEnum.Paid)
                return true; // idempotent – callback gọi nhiều lần không lỗi

            //if (payment.Status == OrderPaymentStatusEnum.Refunded)
            //    throw new AppException("Không thể xác nhận thanh toán cho giao dịch hoàn tiền.");

            payment.Status = OrderPaymentStatusEnum.Paid;
            payment.TransactionCode = transactionCode;
            payment.PaymentDate = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.OrderPayments.Update(payment);
            await _unitOfWork.CompleteAsync();

            await RecalculateOrderPaymentStatusAsync(payment.OrderId);

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

            // Optional: log reason vào bảng riêng hoặc PaymentSignature / Note
            payment.PaymentSignature = reason;

            _unitOfWork.OrderPayments.Update(payment);

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<OrderPaymentReadDto> RefundAsync(
    long orderId,
    decimal amount,
    string reason)
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
                Type = OrderPaymentTypeEnum.refund,
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

            // Lấy tổng tiền đã trả (Sum từ DB)
            var snapshot = await BuildPaymentSnapshotAsync(orderId);

            // Kiểm tra nếu đã trả đủ
            if (snapshot.IsFullyPaid(order.GrandTotal))
            {
                // 3. CẬP NHẬT CỜ ISPAID (Đây là phần bạn cần)
                if (!order.IsPaid) // Chỉ update nếu chưa set để tránh ghi đè ngày cũ
                {
                    order.IsPaid = true;
                    order.PaymentDate = DateTime.UtcNow;

                    // Nếu đơn đang treo -> Tự động Confirm luôn (tùy nghiệp vụ)
                    if (order.Status == OrderStatusEnum.PendingPayment || order.Status == OrderStatusEnum.New)
                    {
                        order.Status = OrderStatusEnum.Confirmed;
                    }

                    _unitOfWork.Orders.Update(order);
                    await _unitOfWork.CompleteAsync(); // Lưu thay đổi của Order
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
            // Tạo bản ghi thanh toán mới (OrderPayment)
            var payment = new OrderPayment
            {
                OrderId = orderId,
                PaymentMethodId = paymentMethodId,
                PaymentMethodName = paymentMethodName,

                Amount = amount,
                TransactionCode = $"AUTO-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}", // Mã tự sinh: AUTO-XXXXXXXX

                // 🟢 Dùng đúng Enum bạn cung cấp
                Status = OrderPaymentStatusEnum.Paid, // 2: Đã thanh toán
                Type = OrderPaymentTypeEnum.charge,   // 1: charge (Thu tiền)

                PaymentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<OrderPayment>().AddAsync(payment);
            await _unitOfWork.CompleteAsync(); // Lưu Payment trước

            // GỌI HÀM NÀY ĐỂ UPDATE IsPaid = TRUE CHO ORDER
            await RecalculateOrderPaymentStatusAsync(orderId);
        }


    }
}
