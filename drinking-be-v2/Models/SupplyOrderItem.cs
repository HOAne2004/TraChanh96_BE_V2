using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class SupplyOrderItem
{
    public long Id { get; set; }

    public long SupplyOrderId { get; set; }

    public int MaterialId { get; set; }

    // --- SỐ LƯỢNG & ĐƠN VỊ ---
    // Nhập theo PurchaseUnit (VD: Nhập 10 Thùng)
    public int Quantity { get; set; }

    // Lưu lại đơn vị nhập lúc đó để đối chiếu (VD: "Thùng")
    public string Unit { get; set; } = null!;

    // --- GIÁ VỐN (SNAPSHOT) ---
    // Giá vốn của 1 đơn vị nhập tại thời điểm đặt (VD: 336k/Thùng)
    // Quan trọng: Giá trong Material có thể đổi, nhưng giá ở đây phải giữ nguyên lịch sử.
    public decimal CostPerUnit { get; set; }

    public decimal TotalCost { get; set; } // = Quantity * CostPerUnit

    // --- NAVIGATION PROPERTIES ---
    public virtual SupplyOrder SupplyOrder { get; set; } = null!;
    public virtual Material Material { get; set; } = null!;
}