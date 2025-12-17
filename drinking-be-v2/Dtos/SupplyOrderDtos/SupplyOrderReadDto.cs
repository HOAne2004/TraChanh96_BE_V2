using System;
using System.Collections.Generic;

namespace drinking_be.Dtos.SupplyOrderDtos
{
    public class SupplyOrderReadDto
    {
        public long Id { get; set; }
        public Guid PublicId { get; set; }
        public string OrderCode { get; set; } = null!;

        // --- Định danh ---
        public int? StoreId { get; set; }
        public string StoreName { get; set; } = null!; // "Kho Tổng" hoặc Tên cửa hàng

        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = null!; // Người tạo

        public int? ApprovedByUserId { get; set; }
        public string? ApprovedByName { get; set; } // Người duyệt

        // --- Tài chính ---
        public decimal TotalAmount { get; set; }

        // --- Trạng thái ---
        public string Status { get; set; } = null!; // Enum String
        public string? Note { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // --- Chi tiết ---
        public ICollection<SupplyOrderItemReadDto> Items { get; set; } = new List<SupplyOrderItemReadDto>();
    }

    // DTO Con: Hiển thị chi tiết món
    public class SupplyOrderItemReadDto
    {
        public long Id { get; set; }
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = null!;
        public string? MaterialImageUrl { get; set; }

        public int Quantity { get; set; }
        public string Unit { get; set; } = null!; // Đơn vị nhập lúc đó (VD: Thùng)

        public decimal CostPerUnit { get; set; } // Giá lúc nhập
        public decimal TotalCost { get; set; }
    }
}