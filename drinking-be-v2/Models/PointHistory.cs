using drinking_be.Enums;
using drinking_be.Interfaces; // Nếu muốn dùng ISoftDelete
using System;

namespace drinking_be.Models;

public class PointHistory
{
    public long Id { get; set; }

    // Liên kết với thẻ thành viên (Thay vì User)
    public long MembershipId { get; set; }

    // Đơn hàng liên quan (Null nếu là điểm Bonus/Reset)
    public long? OrderId { get; set; }

    // Số xu thay đổi (+ hoặc -)
    public int Amount { get; set; }

    // Số dư sau khi thay đổi (Snapshot để đối soát)
    public int BalanceAfter { get; set; }

    // Loại giao dịch: Earning (Tích), Redeeming (Tiêu), Reset (Hết hạn), Adjustment (Admin sửa)
    public string TransactionType { get; set; } = "Earning";

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // --- Navigation ---
    public virtual Membership Membership { get; set; } = null!;
    public virtual Order? Order { get; set; }
}