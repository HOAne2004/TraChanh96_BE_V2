using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.ReviewDtos
{
    public class ReviewCreateDto
    {
        // ⭐️ BỔ SUNG QUAN TRỌNG: Để xác thực verified purchase
        [Required(ErrorMessage = "Mã đơn hàng không được để trống.")]
        public long OrderId { get; set; }

        [Required(ErrorMessage = "Mã sản phẩm không được để trống.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Điểm đánh giá không được để trống.")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5.")]
        public byte Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Nội dung không quá 1000 ký tự.")]
        public string? Content { get; set; }

        public string? MediaUrl { get; set; }
    }
}