// File: Dtos/CategoryDtos/CategoryReadDto.cs

using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.CategoryDtos
{
    public class CategoryReadDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!; // Luôn có Slug

        public byte? SortOrder { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Trạng thái dưới dạng string/label
        public string Status { get; set; } = null!;

        // ⭐ Quan hệ Đệ quy: Cho phép hiển thị danh sách danh mục con
        public ICollection<CategoryReadDto>? Children { get; set; }
    }
}