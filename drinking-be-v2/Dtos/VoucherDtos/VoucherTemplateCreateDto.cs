// File: Dtos/VoucherDtos/VoucherTemplateCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.VoucherDtos
{
    public class VoucherTemplateCreateDto
    {
        [Required(ErrorMessage = "Tên mẫu voucher không được để trống.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Mã Coupon (nếu là Mã công khai)
        [MaxLength(20)]
        public string? CouponCode { get; set; }

        // --- Quy tắc Giảm giá ---
        [Required(ErrorMessage = "Giá trị giảm giá không được để trống.")]
        [Range(0.01, 10000000)]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Loại giảm giá (Ví dụ: Fixed, Percent) không được để trống.")]
        [MaxLength(10)]
        public string DiscountType { get; set; } = string.Empty;

        [Range(0, 100000000)]
        public decimal? MinOrderValue { get; set; } = 0m;

        public decimal? MaxDiscountAmount { get; set; }

        // --- Giới hạn Sử dụng ---
        public int? UsageLimit { get; set; } // Tổng số lần sử dụng tối đa
        public byte? UsageLimitPerUser { get; set; } // Giới hạn mỗi người dùng

        // --- Áp dụng cho Cấp độ Thành viên (Tùy chọn) ---
        public byte? LevelId { get; set; }

        // --- Thời gian hiệu lực ---
        [Required(ErrorMessage = "Ngày bắt đầu hiệu lực không được để trống.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn không được để trống.")]
        public DateTime EndDate { get; set; }

        // Mặc định là Active
        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    }
}