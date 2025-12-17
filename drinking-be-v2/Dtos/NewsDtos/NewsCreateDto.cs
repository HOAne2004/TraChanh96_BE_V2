// File: Dtos/NewsDtos/NewsCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.NewsDtos
{
    public class NewsCreateDto
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung bài viết không được để trống.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại bài viết (ví dụ: Promotion, Blog) không được để trống.")]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }

        // --- SEO & Quản lý ---
        [MaxLength(255)]
        public string? SeoDescription { get; set; }

        public bool? IsFeatured { get; set; } = false;

        // Status ban đầu thường là Draft (Nháp)
        public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Draft;
    }
}