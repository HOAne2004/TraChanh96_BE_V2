namespace drinking_be.Dtos.OrderItemDtos
{
    public class OrderItemReadDto
    {
        public long Id { get; set; }
        public int ProductId { get; set; }

        // --- Snapshot Info ---
        public string ProductName { get; set; } = null!;
        public string? ProductImage { get; set; }

        // --- Price & Quantity ---
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }  // Giá đơn vị (Base + Size) lúc mua
        public decimal TotalPrice { get; set; } // Quantity * UnitPrice

        // --- Options ---
        public string? Note { get; set; }

        public string? SizeName { get; set; } // Tên size (S, M, L)
        public string SugarLevel { get; set; } = null!; // Label (100% Đường)
        public string IceLevel { get; set; } = null!;   // Label (100% Đá)

        // --- Toppings ---
        // 🟢 CẬP NHẬT: Dùng DTO riêng cho topping để gọn nhẹ JSON trả về
        public List<OrderToppingReadDto> Toppings { get; set; } = new();
    }
}