// Models/Order.cs (Cập nhật)
using drinking_be.Enums;
using System;
using System.Collections.Generic;
using drinking_be.Interfaces;

namespace drinking_be.Models;

public partial class Order : ISoftDelete
{
    public long Id { get; set; }

    public string OrderCode { get; set; } = null!;

    public int? UserId { get; set; }
    public int StoreId { get; set; }

    // Giữ lại: Phương thức thanh toán được chọn
    public int? PaymentMethodId { get; set; }

    // ⭐ Bổ sung: Khóa ngoại tới Address (Địa chỉ đã chuẩn hóa)
    public long DeliveryAddressId { get; set; }

    public DateTime? OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? ShippingFee { get; set; }
    public decimal GrandTotal { get; set; }

    public int? CoinsEarned { get; set; }

    public OrderStatusEnum Status { get; set; } = OrderStatusEnum.New;

    public string? VoucherCodeUsed { get; set; }
    public string? StoreName { get; set; }
    public string? UserNotes { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // --- NAVIGATION PROPERTIES ---
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    // ⭐ Bổ sung: Quan hệ One-to-Many tới OrderPayment
    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();

    public virtual PaymentMethod? PaymentMethod { get; set; }
    public virtual Store Store { get; set; } = null!;
    public virtual User? User { get; set; }

    // ⭐ Bổ sung: Quan hệ tới Address (Địa chỉ giao hàng)
    public virtual Address DeliveryAddress { get; set; } = null!;
}