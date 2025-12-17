using System;

namespace drinking_be.Dtos.InventoryDtos
{
    public class InventoryReadDto
    {
        public long Id { get; set; }

        // --- Thông tin Nguyên liệu (Flatten từ Material) ---
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = null!; // Cần Include Material
        public string Unit { get; set; } = null!; // BaseUnit của Material
        public string? MaterialImageUrl { get; set; }
        public int? MinStockAlert { get; set; } // Mức cảnh báo từ Material

        // --- Thông tin Kho (Flatten từ Store) ---
        public int? StoreId { get; set; }
        public string StoreName { get; set; } = null!; // "Kho Tổng" hoặc Tên cửa hàng

        // --- Số liệu ---
        public int Quantity { get; set; }

        // Trạng thái cảnh báo (Tính toán tại thời điểm map: Quantity <= MinStockAlert)
        public bool IsLowStock { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}