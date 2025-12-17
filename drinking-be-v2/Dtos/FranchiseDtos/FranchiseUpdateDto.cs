using drinking_be.Enums;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.FranchiseDtos
{
    public class FranchiseUpdateDto
    {
        // Admin có thể sửa lại thông tin nếu khách điền sai hoặc bổ sung
        public string? FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? TargetArea { get; set; }
        public decimal? EstimatedBudget { get; set; }

        // ⭐ Quan trọng nhất: Cập nhật trạng thái và ghi chú
        public FranchiseStatusEnum? Status { get; set; }
        public string? AdminNote { get; set; }

        // Chuyển hồ sơ cho nhân viên khác phụ trách
        public int? ReviewerId { get; set; }
    }
}