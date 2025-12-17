// File: Dtos/PolicyDtos/PolicyCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.PolicyDtos
{
    public class PolicyCreateDto
    {
        [Required(ErrorMessage = "Mã Brand không được để trống.")]
        public int BrandId { get; set; }

        // StoreId là tùy chọn, cho phép chính sách áp dụng ở cấp Store
        public int? StoreId { get; set; }

        [Required(ErrorMessage = "Tiêu đề chính sách không được để trống.")]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung chính sách không được để trống.")]
        public string Content { get; set; } = string.Empty;

        // Status mặc định là Pending (Chờ duyệt)
        public PolicyReviewStatusEnum Status { get; set; } = PolicyReviewStatusEnum.Pending;

        // Slug sẽ được tạo tự động từ Title trong Service/Mapper
    }
}