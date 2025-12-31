// Models/OrderPayment.cs
using drinking_be.Enums;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class OrderPayment
{
    public long Id { get; set; }

    // Khóa ngoại tới đơn hàng (Bắt buộc)
    public long OrderId { get; set; }

    // Khóa ngoại tới phương thức thanh toán
    public int PaymentMethodId { get; set; }
    public string PaymentMethodName { get; set; } = null!;

    // Thông tin giao dịch
    public decimal Amount { get; set; } // Số tiền của giao dịch này (có thể là tiền cọc, hoàn tiền, hoặc tổng)
    public string? TransactionCode { get; set; } // Mã giao dịch của Ngân hàng/Cổng thanh toán
    public string? PaymentSignature { get; set; } // Chữ ký bảo mật (cần thiết cho các cổng thanh toán)

    // ⭐ Trạng thái (Sử dụng Enum đã tạo)
    public OrderPaymentStatusEnum Status { get; set; } = OrderPaymentStatusEnum.Pending;
    public OrderPaymentTypeEnum Type { get; set; }
    // Charge / Refund / Adjustment

    public DateTime? PaymentDate { get; set; } // Thời điểm thanh toán thành công

    // --- Quản lý ---
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual Order Order { get; set; } = null!;
}