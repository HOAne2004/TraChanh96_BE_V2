// File: Dtos/NewsDtos/NewsReadDto.cs

using drinking_be.Enums;
using drinking_be.Dtos.CommentDtos; // Để hiển thị Comment nếu cần
using System.Collections.Generic;

namespace drinking_be.Dtos.NewsDtos
{
    public class NewsReadDto
    {
        public int Id { get; set; }
        public Guid? PublicId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Slug { get; set; }

        public string? ThumbnailUrl { get; set; }
        public string? SeoDescription { get; set; }
        public bool? IsFeatured { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        public DateTime? PublishedDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Thông tin người tạo (Nếu đã Include User)
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;

        // Navigation Properties
        public ICollection<CommentReadDto> Comments { get; set; } = new List<CommentReadDto>();
    }
}