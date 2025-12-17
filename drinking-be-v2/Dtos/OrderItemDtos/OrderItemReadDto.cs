// File: Dtos/OrderItemDtos/OrderItemReadDto.cs

using drinking_be.Enums;
using System.Collections.Generic;

namespace drinking_be.Dtos.OrderItemDtos
{
    public class OrderItemReadDto
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!; // Cần Include Product
        public int Quantity { get; set; }

        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public string? Note { get; set; }

        // --- Tùy chọn dưới dạng Label/String ---
        public short? SizeId { get; set; }
        public string? SizeLabel { get; set; } // Cần Include Size

        public string SugarLabel { get; set; } = null!; // Lấy từ Enum.ToString()
        public string IceLabel { get; set; } = null!;   // Lấy từ Enum.ToString()

        // ⭐ Quan hệ đệ quy: Topping (Cũng là OrderItem, nhưng dùng DTO đơn giản hơn)
        public ICollection<OrderToppingReadDto> Toppings { get; set; } = new List<OrderToppingReadDto>();
    }
}