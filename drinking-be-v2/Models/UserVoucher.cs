using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;

public partial class UserVoucher : ISoftDelete
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public int VoucherTemplateId { get; set; }

    // Nullable – chỉ dùng cho voucher cá nhân
    public string? VoucherCode { get; set; }

    public DateTime IssuedDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public UserVoucherStatusEnum Status { get; set; } = UserVoucherStatusEnum.Unused;

    public DateTime? UsedDate { get; set; }

    public long? OrderIdUsed { get; set; }

    public DateTime? DeletedAt { get; set; }

    // --- Navigation ---
    public virtual User User { get; set; } = null!;

    public virtual VoucherTemplate VoucherTemplate { get; set; } = null!;

    public virtual Order? OrderUsed { get; set; }
}
