// File: Dtos/CartDtos/CartItemReadDto.cs

using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.CartDtos
{
    public class CartItemReadDto
    {
        public long Id { get; set; }
        public long CartId { get; set; }
        public int ProductId { get; set; }

        // Thông tin Product (Cần Include Product)
        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; } // Giả định DTO cần ImageUrl

        public int Quantity { get; set; }

        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public string? Note { get; set; }

        // --- Tùy chọn dưới dạng Label/String ---
        public short? SizeId { get; set; }
        public string? SizeLabel { get; set; } // Cần Include Size

        // ⭐ Lấy từ Enum.ToString()
        public string? SugarLabel { get; set; }
        public string? IceLabel { get; set; }

        // ⭐ Quan hệ đệ quy: Topping (dùng DTO đơn giản hơn)
        public ICollection<CartToppingReadDto> Toppings { get; set; } = new List<CartToppingReadDto>();
    }
}