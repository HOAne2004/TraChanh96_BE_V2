using drinking_be.Dtos.OrderItemDtos;
using drinking_be.Dtos.OrderPaymentDtos;
using drinking_be.Dtos.PaymentMethodDtos;
using drinking_be.Enums;

namespace drinking_be.Dtos.OrderDtos
{
    public class OrderReadDto
    {
        public long Id { get; set; }
        public string OrderCode { get; set; } = null!;

        // 🟢 [SỬA 1] Đổi từ string sang Enum để logic FE so sánh bằng số (0, 1, 2...)
        public OrderTypeEnum OrderType { get; set; }
        // 🟢 [THÊM] Label hiển thị tiếng Việt
        public string OrderTypeLabel { get; set; } = string.Empty;

        // 🟢 [GIỮ NGUYÊN] Đã đúng (Enum cho logic, String cho hiển thị)
        public OrderStatusEnum Status { get; set; }
        public string StatusLabel { get; set; } = null!;

        // --- Store Info ---
        public int StoreId { get; set; }
        public string StoreName { get; set; } = null!;

        // --- User Info ---
        public int? UserId { get; set; }
        public string UserName { get; set; } = "Khách vãng lai";

        // --- Delivery Snapshot ---
        public string? RecipientName { get; set; }
        public string? RecipientPhone { get; set; }
        public string? ShippingAddress { get; set; }

        // --- At Counter Info ---
        public string? PickupCode { get; set; }
        public int? TableId { get; set; }
        public string? TableName { get; set; }

        // --- Shipper Info ---
        public int? ShipperId { get; set; }
        public string? ShipperName { get; set; }
        public string? ShipperPhone { get; set; }

        // --- Financials ---
        public decimal TotalAmount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public int? CoinsEarned { get; set; }

        // --- Payment ---
        public string? PaymentMethodName { get; set; }
        public PaymentMethodReadDto? PaymentMethod { get; set; }
        public bool IsPaid { get; set; }

        // --- Meta ---
        public string? UserNotes { get; set; }
        public string? VoucherCodeUsed { get; set; }

        // --- Timestamps ---
        public DateTime CreatedAt { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        // --- Cancellation ---
        // 🟢 [SỬA 2] Đổi từ string sang Enum (để FE so sánh lý do)
        public OrderCancelReasonEnum? CancelReason { get; set; }
        // 🟢 [THÊM] Label hiển thị tiếng Việt cho lý do hủy
        public string? CancelReasonLabel { get; set; }

        public string? CancelNote { get; set; }
        public int? CancelledByUserId { get; set; }

        // --- Collections ---
        public List<OrderItemReadDto> Items { get; set; } = new();
        public List<OrderPaymentReadDto> Payments { get; set; } = new();
    }
}