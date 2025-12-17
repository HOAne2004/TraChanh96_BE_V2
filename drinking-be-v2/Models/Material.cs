using drinking_be.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using drinking_be.Interfaces;

namespace drinking_be.Models;

public partial class Material: ISoftDelete
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }

    [Required]
    public string Name { get; set; } = null!; // "Sữa tươi Vinamilk"

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    // --- QUY ĐỔI ĐƠN VỊ ---

    [Required]
    public string BaseUnit { get; set; } = null!; // Ví dụ: "Hộp" (Dùng để đếm tồn kho)

    public string? PurchaseUnit { get; set; } // Ví dụ: "Thùng" (Dùng để nhập hàng)

    public int ConversionRate { get; set; } = 1; // Ví dụ: 12 (1 Thùng = 12 Hộp)

    // --- GIÁ VỐN ---

    // Giá nhập cho 1 đơn vị nhập (Ví dụ: 336.000đ / Thùng)
    // Đây là giá niêm yết hiện tại
    public decimal CostPerPurchaseUnit { get; set; }

    // Giá vốn cho 1 đơn vị cơ sở (Tự động tính: 336k / 12 = 28k / Hộp)
    // Thuộc tính này có thể computed (tính toán) hoặc lưu trữ để tiện truy vấn
    [NotMapped] // Không cần tạo cột trong DB, tính toán ở code
    public decimal CostPerBaseUnit => ConversionRate > 0 ? CostPerPurchaseUnit / ConversionRate : 0;

    // Mức cảnh báo tồn kho (Tính theo BaseUnit - Hộp)
    public int? MinStockAlert { get; set; }

    public bool IsActive { get; set; } = true;

    // ... (Audit & Navigation Props)
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual ICollection<SupplyOrderItem> SupplyOrderItems { get; set; } = new List<SupplyOrderItem>();
}