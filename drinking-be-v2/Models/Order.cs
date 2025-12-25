using drinking_be.Enums;
using drinking_be.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace drinking_be.Models;

public partial class Order : ISoftDelete
{
    public long Id { get; set; }

    // --- ĐỊNH DANH & PHÂN LOẠI ---
    public string OrderCode { get; set; } = null!; // Mã đơn hệ thống (Unique)
    public string? PickupCode { get; set; }        // Mã lấy đồ/giao hàng (Ngắn gọn)
    public OrderTypeEnum OrderType { get; set; } = OrderTypeEnum.AtCounter;
    public OrderStatusEnum Status { get; set; } = OrderStatusEnum.New;

    // --- KHÓA NGOẠI (FOREIGN KEYS) ---
    public int StoreId { get; set; }
    public int? UserId { get; set; }            // Khách hàng
    public int? ShipperId { get; set; }         // Nhân viên giao hàng (User)
    public int? TableId { get; set; }           // Bàn (ShopTable)
    public int? PaymentMethodId { get; set; }
    public long? DeliveryAddressId { get; set; }
    public long? UserVoucherId { get; set; }

    // --- THỜI GIAN ---
    public DateTime? OrderDate { get; set; }    // Thời điểm đặt
    public DateTime? DeliveryDate { get; set; } // Thời điểm giao xong
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // --- TÀI CHÍNH (MONEY) ---
    // (Lưu ý: Precision sẽ được config trong DbContext)
    public decimal TotalAmount { get; set; }    // Tổng tiền hàng
    public decimal? DiscountAmount { get; set; } // Giảm giá
    public decimal? ShippingFee { get; set; }    // Phí ship
    public decimal GrandTotal { get; set; }      // Tổng thanh toán cuối cùng
    public int? CoinsEarned { get; set; }        // Xu tích lũy

    // --- THÔNG TIN BỔ SUNG ---
    public string? StoreName { get; set; }       // Snapshot tên cửa hàng lúc đặt
    public string? VoucherCodeUsed { get; set; } // Snapshot mã voucher
    public string? UserNotes { get; set; }       // Ghi chú của khách

    // --- THÔNG TIN HỦY ĐƠN ---
    public OrderCancelReasonEnum? CancelReason { get; set; }
    public string? CancelNote { get; set; }
    public int? CancelledByUserId { get; set; }

    // =========================================================
    // NAVIGATION PROPERTIES
    // =========================================================

    // 1. Quan hệ chính
    public virtual Store Store { get; set; } = null!;
    public virtual User? User { get; set; }      // Khách hàng
    public virtual User? Shipper { get; set; }   // Shipper (Cũng là User)
    public virtual ShopTable? Table { get; set; } // Bàn

    // 2. Thanh toán & Voucher & Địa chỉ
    public virtual PaymentMethod? PaymentMethod { get; set; }
    public virtual UserVoucher? UserVoucher { get; set; }
    public virtual Address? DeliveryAddress { get; set; }

    // 3. Danh sách con
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
}