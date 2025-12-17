using System.ComponentModel.DataAnnotations;

namespace drinking_be.Dtos.CartDtos
{
    public class CartItemCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "Số lượng phải ít nhất là 1.")]
        public int Quantity { get; set; }

        [Required]
        public short SizeId { get; set; }

        public short? SugarLevelId { get; set; }
        public short? IceLevelId { get; set; }

        public string? Note { get; set; }

        // Danh sách topping (nếu có)
        public List<CartToppingCreateDto>? Toppings { get; set; }
    }

    public class CartToppingCreateDto
    {
        public int ProductId { get; set; } // ID của sản phẩm Topping
        public int Quantity { get; set; } // Số lượng topping trên 1 ly
    }
}