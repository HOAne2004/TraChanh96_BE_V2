using drinking_be.Enums;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.ProductDtos
{
    // Dùng cho API Admin quản lý cửa hàng hoặc Store Manager
    // Update trạng thái món ăn tại cửa hàng cụ thể
    public class ProductStoreUpdateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int StoreId { get; set; }

        [Required]
        public ProductStoreStatusEnum Status { get; set; }

    }

    public class ProductStoreReadDto
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public int SoldCount { get; set; }
        public string Status { get; set; } = string.Empty; // Label
        public decimal? PriceOverride { get; set; }
    }

    public class StoreMenuReadDto : ProductReadDto
    {
        // --- Các trường bổ sung cho riêng Cửa hàng ---

        public bool IsSoldOut { get; set; }

        public string StoreStatus { get; set; } = null!;

        public decimal DisplayPrice { get; set; }
    }
}