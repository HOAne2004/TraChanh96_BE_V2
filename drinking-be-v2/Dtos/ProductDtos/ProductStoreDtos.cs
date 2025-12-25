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

        // Không có PriceOverride vì bạn quyết định Store Manager không được sửa giá
    }

    public class ProductStoreReadDto
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public int SoldCount { get; set; }
        public string Status { get; set; } = string.Empty; // Label
        public decimal? PriceOverride { get; set; }
    }

    // Kế thừa từ ProductReadDto để tận dụng các field cơ bản (Name, Image, BasePrice...)
    public class StoreMenuReadDto : ProductReadDto
    {
        // --- Các trường bổ sung cho riêng Cửa hàng ---

        // Cờ đánh dấu để Frontend làm mờ (True nếu ProductStore.Status == OutOfStock)
        public bool IsSoldOut { get; set; }

        // Trạng thái cụ thể tại cửa hàng (Available, OutOfStock, Disabled...)
        public string StoreStatus { get; set; } = null!;

        // Giá hiển thị tại cửa hàng (Nếu có PriceOverride thì lấy, không thì lấy BasePrice)
        // Frontend sẽ dùng giá này để hiển thị chính
        public decimal DisplayPrice { get; set; }
    }
}