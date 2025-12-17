// File: Dtos/OrderDtos/OrderReadDto.cs

using drinking_be.Enums;
using drinking_be.Dtos.OrderItemDtos;
using drinking_be.Dtos.AddressDtos;
using drinking_be.Dtos.PaymentMethodDtos;
using System.Collections.Generic;

namespace drinking_be.Dtos.OrderDtos
{
    public class OrderReadDto
    {
        public long Id { get; set; }
        public string OrderCode { get; set; } = null!;
        public Guid PublicId { get; set; } // Giả định có PublicId (nên có)

        // --- Liên kết ---
        public int? UserId { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; } = null!; // Cần Include Store

        // --- Thanh toán ---
        public PaymentMethodReadDto PaymentMethod { get; set; } = null!; // Cần Include PaymentMethod

        // ⭐ Địa chỉ giao hàng (Cần Include DeliveryAddress)
        public AddressReadDto DeliveryAddress { get; set; } = null!;

        // --- Tài chính ---
        public decimal TotalAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal GrandTotal { get; set; }

        public int? CoinsEarned { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;
        public string? VoucherCodeUsed { get; set; }
        public string? UserNotes { get; set; }

        // --- Thời gian ---
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? CreatedAt { get; set; }

        // ⭐ Order Items (Sẽ được ánh xạ chi tiết)
        public ICollection<OrderItemReadDto> Items { get; set; } = new List<OrderItemReadDto>();

        // ⭐ Order Payments (Lịch sử thanh toán, nếu có)
        // public ICollection<OrderPaymentReadDto> Payments { get; set; } = new List<OrderPaymentReadDto>();
    }
}