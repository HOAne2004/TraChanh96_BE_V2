// File: Dtos/ProductDtos/ProductUpdateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.ProductDtos
{
    public class ProductUpdateDto
    {
        [MaxLength(255)]
        public string? Name { get; set; }

        public int? CategoryId { get; set; }
        public string? Slug { get; set; } = null!;

        [MaxLength(20)]
        public string? ProductType { get; set; }

        [Range(0.01, 10000000)]
        public decimal? BasePrice { get; set; }

        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? Ingredient { get; set; }

        public ProductStatusEnum? Status { get; set; }

        public DateTime? LaunchDateTime { get; set; }

        // ⭐ Cập nhật các liên kết Size: Nếu gửi list này, Service sẽ xóa và tạo lại các ProductSize
        public ICollection<ProductSizeCreateDto>? ProductSizes { get; set; }
        public List<int>? AllowedToppingIds { get; set; }
    }
}