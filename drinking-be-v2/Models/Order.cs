using drinking_be.Enums;
using drinking_be.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drinking_be.Models;

public partial class Order : ISoftDelete
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    // --- ĐỊNH DANH & PHÂN LOẠI ---
    [Required]
    [MaxLength(50)]
    public string OrderCode { get; set; } = null!;

    [MaxLength(20)]
    public string? PickupCode { get; set; }
    public DateTime? PickupTime { get; set; } // Thời gian khách hẹn lấy
    public OrderTypeEnum OrderType { get; set; } = OrderTypeEnum.AtCounter;
    public OrderStatusEnum Status { get; set; } = OrderStatusEnum.New;

    // --- KHÓA NGOẠI (FOREIGN KEYS) ---
    public int StoreId { get; set; }
    public int? UserId { get; set; }
    public int? ShipperId { get; set; }
    public int? TableId { get; set; }
    public int? PaymentMethodId { get; set; }
    public long? DeliveryAddressId { get; set; }
    public long? UserVoucherId { get; set; }

    // --- SNAPSHOT GIAO HÀNG ---
    [MaxLength(100)]
    public string? RecipientName { get; set; }

    [MaxLength(20)]
    public string? RecipientPhone { get; set; }

    [MaxLength(500)]
    public string? ShippingAddress { get; set; }

    // --- THỜI GIAN ---
    public DateTime? OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // --- TÀI CHÍNH (MONEY) ---
    // Vẫn giữ lại cấu hình Decimal để tránh lỗi database

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ShippingFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal GrandTotal { get; set; } = 0;

    public bool IsPaid { get; set; } = false;
    public DateTime? PaymentDate { get; set; }
    public int? CoinsEarned { get; set; } = 0;

    // --- THÔNG TIN BỔ SUNG ---
    [MaxLength(255)]
    public string? StoreName { get; set; }

    [MaxLength(50)]
    public string? PaymentMethodName { get; set; }

    [MaxLength(50)]
    public string? VoucherCodeUsed { get; set; }

    [MaxLength(1000)]
    public string? UserNotes { get; set; }

    // --- THÔNG TIN HỦY ĐƠN ---
    public OrderCancelReasonEnum? CancelReason { get; set; }

    [MaxLength(500)]
    public string? CancelNote { get; set; }
    public int? CancelledByUserId { get; set; }

    // =========================================================
    // NAVIGATION PROPERTIES
    // =========================================================

    [ForeignKey(nameof(StoreId))]
    public virtual Store Store { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }

    [ForeignKey(nameof(ShipperId))]
    public virtual User? Shipper { get; set; }

    [ForeignKey(nameof(TableId))]
    public virtual ShopTable? Table { get; set; }

    [ForeignKey(nameof(PaymentMethodId))]
    public virtual PaymentMethod? PaymentMethod { get; set; }

    [ForeignKey(nameof(UserVoucherId))]
    public virtual UserVoucher? UserVoucher { get; set; }

    [ForeignKey(nameof(DeliveryAddressId))]
    public virtual Address? DeliveryAddress { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}