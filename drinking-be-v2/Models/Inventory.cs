using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace drinking_be.Models;

public partial class Inventory
{
    public long Id { get; set; }

    // --- KHÓA NGOẠI ---
    public int MaterialId { get; set; }

    // Nếu StoreId = NULL => Đây là KHO TỔNG (Admin giữ)
    // Nếu StoreId có giá trị => Đây là KHO CỬA HÀNG
    public int? StoreId { get; set; }

    // --- SỐ LIỆU ---
    public int Quantity { get; set; } = 0; // Số lượng tồn kho hiện tại

    public DateTime LastUpdated { get; set; } // Thời điểm cập nhật cuối cùng

    // Navigation Properties
    public virtual Material Material { get; set; } = null!;
    public virtual Store? Store { get; set; }
}