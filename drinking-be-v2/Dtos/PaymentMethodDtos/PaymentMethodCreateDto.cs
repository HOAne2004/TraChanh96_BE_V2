// File: Dtos/PaymentMethodDtos/PaymentMethodCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.PaymentMethodDtos
{
    public class PaymentMethodCreateDto
    {
        [Required(ErrorMessage = "Tên phương thức thanh toán không được để trống.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại thanh toán không được để trống.")]
        public PaymentTypeEnum PaymentType { get; set; } = PaymentTypeEnum.COD;

        public string? ImageUrl { get; set; }

        // --- Thông tin chi tiết cho Chuyển khoản/Ví (Nếu PaymentType != COD) ---
        [MaxLength(100)]
        public string? BankName { get; set; }

        [MaxLength(50)]
        public string? BankAccountNumber { get; set; }

        [MaxLength(100)]
        public string? BankAccountName { get; set; }

        [MaxLength(500)]
        public string? Instructions { get; set; }
        public string? QRTplUrl { get; set; }

        [Range(0, 100)] // Giả sử phí xử lý tối đa 100%
        public decimal? ProcessingFee { get; set; } = 0m;

        public byte? SortOrder { get; set; }

        // Mặc định là Active
        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}