// File: Dtos/VoucherDtos/UserVoucherReadDto.cs

using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.VoucherDtos
{
    public class UserVoucherReadDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public int VoucherTemplateId { get; set; }

        // --- Thông tin Voucher ---
        public string VoucherCode { get; set; } = null!;
        public DateTime? IssuedDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime? UsedDate { get; set; }
        public long? OrderIdUsed { get; set; }

        // --- Thông tin từ Template (Cần Include VoucherTemplate) ---
        public string TemplateName { get; set; } = null!;
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = null!;
        public decimal? MinOrderValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
    }
}