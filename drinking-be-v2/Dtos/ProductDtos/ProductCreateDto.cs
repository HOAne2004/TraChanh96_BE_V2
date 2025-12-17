// File: Dtos/ProductDtos/ProductCreateDto.cs

using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.ProductDtos
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã Danh mục không được để trống.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Loại sản phẩm (Ví dụ: Beverage, Topping) không được để trống.")]
        [MaxLength(20)]
        public string ProductType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá cơ bản không được để trống.")]
        [Range(0.01, 10000000, ErrorMessage = "Giá phải lớn hơn 0.")]
        public decimal BasePrice { get; set; }

        // --- Thông tin mô tả ---
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? Ingredient { get; set; }

        // --- Quản lý & Trạng thái ---
        public ProductStatusEnum Status { get; set; } = ProductStatusEnum.Active;
        public DateTime? LaunchDateTime { get; set; }

        // ⭐ CÁC LIÊN KẾT BAN ĐẦU: ID các Size áp dụng cho sản phẩm này
        // (Sẽ được xử lý trong Service Layer để tạo ProductSize entities)
        public ICollection<short>? SizeIds { get; set; }
    }
}