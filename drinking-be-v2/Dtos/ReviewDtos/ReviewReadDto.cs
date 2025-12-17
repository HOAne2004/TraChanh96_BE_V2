// File: Dtos/ReviewDtos/ReviewReadDto.cs

using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.ReviewDtos
{
    public class ReviewReadDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!; // Cần Include Product

        // --- Thông tin User ---
        public int UserId { get; set; }
        public string UserName { get; set; } = null!; // Cần Include User
        public string? UserThumbnailUrl { get; set; }

        // --- Nội dung đánh giá ---
        public string? Content { get; set; }
        public byte Rating { get; set; }
        public string? MediaUrl { get; set; }

        // --- Phản hồi & Trạng thái ---
        public string Status { get; set; } = null!; // Trạng thái kiểm duyệt (String/Label)
        public string? AdminResponse { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}