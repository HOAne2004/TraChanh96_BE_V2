namespace drinking_be.Dtos.MaterialDtos
{
    public class MaterialReadDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; } // Dùng định danh cho API

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // --- Đơn vị ---
        public string BaseUnit { get; set; } = null!;
        public string? PurchaseUnit { get; set; }
        public int ConversionRate { get; set; }

        // --- Tài chính ---
        public decimal CostPerPurchaseUnit { get; set; }

        // ⭐ Giá vốn cơ sở (VD: Giá 1 hộp) - Quan trọng để tính COGS (Cost of Goods Sold)
        public decimal CostPerBaseUnit { get; set; }

        public int? MinStockAlert { get; set; }
        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}