// File: Dtos/VoucherDtos/VoucherTemplateUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.VoucherDtos
{
    public class VoucherTemplateUpdateDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(20)]
        public string? CouponCode { get; set; }

        // --- Quy tắc Giảm giá ---
        [Range(0.01, 10000000)]
        public decimal? DiscountValue { get; set; }

        [MaxLength(10)]
        public string? DiscountType { get; set; }

        [Range(0, 100000000)]
        public decimal? MinOrderValue { get; set; }

        public decimal? MaxDiscountAmount { get; set; }

        // --- Giới hạn Sử dụng ---
        public int? UsageLimit { get; set; }
        public int? UsedCount { get; set; }
        public byte? UsageLimitPerUser { get; set; }

        // --- Áp dụng cho Cấp độ Thành viên (Tùy chọn) ---
        public byte? LevelId { get; set; }

        // --- Thời gian hiệu lực ---
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public PublicStatusEnum? Status { get; set; }
    }
}