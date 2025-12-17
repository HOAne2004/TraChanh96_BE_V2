using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.FranchiseDtos
{
    public class FranchiseCreateDto
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Khu vực dự định mở không được để trống.")]
        [MaxLength(200)]
        public string TargetArea { get; set; } = string.Empty;

        [Range(0, 100000000000, ErrorMessage = "Ngân sách không hợp lệ.")]
        public decimal? EstimatedBudget { get; set; }

        public string? ExperienceDescription { get; set; }

        // Status mặc định là Pending, User không được quyền set
    }
}