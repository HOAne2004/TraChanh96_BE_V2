// File: Dtos/CategoryDtos/CategoryUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.CategoryDtos
{
    public class CategoryUpdateDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        // ⭐ Cho phép cập nhật ParentId
        public int? ParentId { get; set; }

        public byte? SortOrder { get; set; }

        public PublicStatusEnum? Status { get; set; }

        // ⭐ Slug chỉ nên được cập nhật nếu tên thay đổi
        public string? Slug { get; set; }
    }
}