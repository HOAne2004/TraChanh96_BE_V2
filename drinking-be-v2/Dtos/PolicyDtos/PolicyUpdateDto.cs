// File: Dtos/PolicyDtos/PolicyUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.PolicyDtos
{
    public class PolicyUpdateDto
    {
        [MaxLength(100)]
        public string? Title { get; set; }

        public string? Content { get; set; }

        // Admin có thể cập nhật Slug (ít phổ biến)
        public string? Slug { get; set; }

        // Cập nhật trạng thái kiểm duyệt
        public PolicyReviewStatusEnum? Status { get; set; }

        // Có thể thay đổi áp dụng chính sách cho Store khác
        public int? StoreId { get; set; }
    }
}