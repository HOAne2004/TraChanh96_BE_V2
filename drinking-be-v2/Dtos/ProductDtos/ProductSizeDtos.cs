using System.ComponentModel.DataAnnotations;
using drinking_be.Enums;

namespace drinking_be.Dtos.ProductDtos
{
    // Dùng khi tạo mới sản phẩm (Kèm cấu hình size)
    public class ProductSizeCreateDto
    {
        [Required]
        public short SizeId { get; set; }

        // Cho phép set giá riêng ngay lúc tạo (Optional)
        [Range(0, 10000000)]
        public decimal? PriceOverride { get; set; }
    }

    // Dùng khi đọc dữ liệu ra (Hiển thị Menu)
    public class ProductSizeReadDto
    {
        public short SizeId { get; set; }
        public string SizeLabel { get; set; } = string.Empty; // VD: "L", "M"

        // Giá gốc của Size (Lấy từ bảng Size)
        public decimal SizeModifierPrice { get; set; }

        // Giá ghi đè (Lấy từ bảng ProductSize)
        public decimal? PriceOverride { get; set; }

        // Giá cuối cùng user phải trả (Logic tính toán sẵn cho FE đỡ khổ)
        // Final = PriceOverride ?? (BasePrice + SizeModifierPrice)
        public decimal FinalPrice { get; set; }

        public string Status { get; set; } = "Active";
    }
}