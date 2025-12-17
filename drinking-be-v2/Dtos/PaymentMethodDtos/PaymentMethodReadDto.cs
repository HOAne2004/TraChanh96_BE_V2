// File: Dtos/PaymentMethodDtos/PaymentMethodReadDto.cs

using drinking_be.Enums;

namespace drinking_be.Dtos.PaymentMethodDtos
{
    public class PaymentMethodReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }

        // Loại thanh toán dưới dạng string/label
        public string PaymentType { get; set; } = null!;

        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }
        public string? Instructions { get; set; }
        public string? QRTplUrl { get; set; }

        public decimal? ProcessingFee { get; set; }
        public byte? SortOrder { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}