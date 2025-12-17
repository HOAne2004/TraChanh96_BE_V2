// File: Dtos/ReviewDtos/ReviewCreateDto.cs

using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.ReviewDtos
{
    public class ReviewCreateDto
    {
        // UserId sẽ được Service Layer lấy từ token (Ignore trong mapping)
        // public int UserId { get; set; } 

        [Required(ErrorMessage = "Mã sản phẩm không được để trống.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Điểm đánh giá không được để trống.")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5.")]
        public byte Rating { get; set; }

        [MaxLength(1000)]
        public string? Content { get; set; }

        public string? MediaUrl { get; set; } // Ảnh/Video đính kèm

        // Status mặc định là Pending
    }
}