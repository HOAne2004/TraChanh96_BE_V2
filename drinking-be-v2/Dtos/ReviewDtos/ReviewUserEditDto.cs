using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.ReviewDtos
{
    // DTO này dùng cho API: PUT /api/reviews/{id} (Chỉ user sở hữu mới sửa được)
    public class ReviewUserEditDto
    {
        [Range(1, 5)]
        public byte Rating { get; set; }

        [MaxLength(1000)]
        public string? Content { get; set; }

        public string? MediaUrl { get; set; }
    }
}