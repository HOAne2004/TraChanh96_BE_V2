// File: Dtos/OrderPaymentDtos/OrderPaymentReadDto.cs

using drinking_be.Enums;
using drinking_be.Dtos.PaymentMethodDtos;

namespace drinking_be.Dtos.OrderPaymentDtos
{
    public class OrderPaymentReadDto
    {
        public long Id { get; set; }
        public long OrderId { get; set; }

        // --- Thông tin Phương thức Thanh toán ---
        public int PaymentMethodId { get; set; }
        public PaymentMethodReadDto PaymentMethod { get; set; } = null!; // Cần Include PaymentMethod

        // --- Thông tin giao dịch ---
        public decimal Amount { get; set; }
        public string? TransactionCode { get; set; }

        // ⭐ Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime? PaymentDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}