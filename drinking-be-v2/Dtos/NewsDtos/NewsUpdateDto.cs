// File: Dtos/NewsDtos/NewsUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.NewsDtos
{
    public class NewsUpdateDto
    {
        [MaxLength(255)]
        public string? Title { get; set; }

        public string? Content { get; set; }

        [MaxLength(50)]
        public string? Type { get; set; }

        public string? ThumbnailUrl { get; set; }

        [MaxLength(255)]
        public string? SeoDescription { get; set; }

        public bool? IsFeatured { get; set; }

        // Cho phép cập nhật Slug (ít khi dùng, thường được Service tính toán)
        public string? Slug { get; set; }

        // Cập nhật trạng thái (Published, Draft, Archived)
        public ContentStatusEnum? Status { get; set; }

        // Có thể update PublishedDate
        public DateTime? PublishedDate { get; set; }
    }
}