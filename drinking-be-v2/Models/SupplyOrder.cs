using drinking_be.Enums;
using drinking_be.Interfaces;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class SupplyOrder : ISoftDelete
{
    public long Id { get; set; }

    public Guid PublicId { get; set; } // Dùng cho API

    // Mã phiếu (VD: SO-20231027-001) - Sinh tự động ở Service
    public string OrderCode { get; set; } = null!;

    // --- ĐỊNH DANH ---
    // Cửa hàng nào yêu cầu nhập? (Nếu Null => Kho tổng nhập từ Nhà cung cấp ngoài)
    public int? StoreId { get; set; }

    public int? SupplierId { get; set; } // (Mở rộng: Nếu nhập từ NCC ngoài) - Tạm thời có thể bỏ qua hoặc để null

    // Người tạo phiếu (Manager)
    public int CreatedByUserId { get; set; }

    // Người duyệt phiếu (Admin) - Null khi mới tạo
    public int? ApprovedByUserId { get; set; }

    // --- TÀI CHÍNH ---
    public decimal TotalAmount { get; set; } = 0; // Tổng tiền vốn

    // --- TRẠNG THÁI & THỜI GIAN ---
    public SupplyOrderStatusEnum Status { get; set; } = SupplyOrderStatusEnum.Pending;
    public string? Note { get; set; } // Ghi chú (VD: "Giao gấp")

    public DateTime? ExpectedDeliveryDate { get; set; } // Ngày mong muốn nhận
    public DateTime? ReceivedAt { get; set; } // Ngày thực tế nhận (quan trọng để chốt sổ)

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; } // Soft Delete

    // --- NAVIGATION PROPERTIES ---
    public virtual Store? Store { get; set; }
    public virtual User CreatedBy { get; set; } = null!;
    public virtual User? ApprovedBy { get; set; }

    public virtual ICollection<SupplyOrderItem> SupplyOrderItems { get; set; } = new List<SupplyOrderItem>();
}