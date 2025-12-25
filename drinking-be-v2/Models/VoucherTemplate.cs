using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;

public partial class VoucherTemplate : ISoftDelete
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    // Nullable = voucher public
    public int? MembershipLevelId { get; set; }

    // --- Discount ---
    public VoucherDiscountTypeEnum DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public decimal? MinOrderValue { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    // --- Usage rules ---
    public int? TotalQuantity { get; set; }

    public int? UsedCount { get; set; }

    public int? UsageLimitPerUser { get; set; }

    // --- Code ---
    public string? CouponCode { get; set; }

    // --- Lifecycle ---
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

    // --- Audit ---
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // --- Navigation ---
    public virtual MembershipLevel? MembershipLevel { get; set; }

    public virtual ICollection<UserVoucher> UserVouchers { get; set; } = new List<UserVoucher>();
}
