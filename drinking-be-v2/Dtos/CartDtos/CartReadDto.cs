// File: Dtos/CartDtos/CartReadDto.cs

using System.Collections.Generic;

namespace drinking_be.Dtos.CartDtos
{
    public class CartReadDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }

        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // ⭐ THÔNG TIN TÍNH TOÁN (Service Layer tính toán)
        public decimal TotalAmount { get; set; } = 0m;

        // Danh sách các món hàng trong giỏ (Cần Include CartItems)
        public ICollection<CartItemReadDto> Items { get; set; } = new List<CartItemReadDto>();
    }
}