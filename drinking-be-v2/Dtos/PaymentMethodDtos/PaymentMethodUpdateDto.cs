// File: Dtos/PaymentMethodDtos/PaymentMethodUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.PaymentMethodDtos
{
    public class PaymentMethodUpdateDto
    {
        [MaxLength(50)]
        public string? Name { get; set; }

        public string? ImageUrl { get; set; }

        public PaymentTypeEnum? PaymentType { get; set; }

        [MaxLength(100)]
        public string? BankName { get; set; }

        [MaxLength(50)]
        public string? BankAccountNumber { get; set; }

        [MaxLength(100)]
        public string? BankAccountName { get; set; }

        [MaxLength(500)]
        public string? Instructions { get; set; }
        public string? QRTplUrl { get; set; }

        [Range(0, 100)]
        public decimal? ProcessingFee { get; set; }

        public byte? SortOrder { get; set; }

        public PublicStatusEnum? Status { get; set; }
    }
}