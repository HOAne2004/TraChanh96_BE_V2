// File: Dtos/CategoryDtos/CategoryCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.CategoryDtos
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // ParentId cho danh mục con (Nếu ParentId là null, đó là danh mục gốc)
        public int? ParentId { get; set; }

        public byte? SortOrder { get; set; } = 0;

        // Admin có thể thiết lập trạng thái ban đầu
        public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
        
        // ⭐ Slug sẽ được tính toán trong Service Layer hoặc AutoMapper
        public string? Slug { get; set; }
    }
}