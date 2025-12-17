// File: Dtos/VoucherDtos/VoucherTemplateReadDto.cs

using drinking_be.Enums;

namespace drinking_be.Dtos.VoucherDtos
{
    public class VoucherTemplateReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? CouponCode { get; set; }

        // --- Quy tắc ---
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = null!; // String/Label
        public decimal? MinOrderValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }

        // --- Giới hạn ---
        public int? UsageLimit { get; set; }
        public int? UsedCount { get; set; } // Số lần đã sử dụng
        public byte? UsageLimitPerUser { get; set; }

        // --- Thành viên ---
        public byte? LevelId { get; set; }
        public string? LevelName { get; set; } // Cần Include MembershipLevel

        // --- Thời gian & Trạng thái ---
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!; // String/Label

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}