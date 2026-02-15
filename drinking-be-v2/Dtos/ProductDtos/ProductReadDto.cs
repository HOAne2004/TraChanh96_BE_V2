// File: Dtos/ProductDtos/ProductReadDto.cs

using drinking_be.Dtos.SizeDtos; // Cho các tùy chọn Size
using drinking_be.Enums;

namespace drinking_be.Dtos.ProductDtos
{
    public class ProductReadDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;

        // --- Giá và Phân loại ---
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!; // Cần Include Category
        public string ProductType { get; set; } = null!;
        public decimal BasePrice { get; set; }

        // --- Mô tả & Ảnh ---
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? Ingredient { get; set; }

        // --- Thống kê & Trạng thái ---
        public string Status { get; set; } = null!; // String/Label
        public double? TotalRating { get; set; }
        public int? TotalSold { get; set; }
        public DateTime? LaunchDateTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<ProductSizeReadDto> ProductSizes { get; set; } = new List<ProductSizeReadDto>();
        public List<int> StoreIds { get; set; } = new List<int>();
        public List<int> AllowedToppingIds { get; set; } = new List<int>();
        public string CategorySlug { get; set; } = string.Empty;
    }
}