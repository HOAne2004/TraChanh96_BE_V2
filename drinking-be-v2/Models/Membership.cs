using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;

public partial class Membership : ISoftDelete
{
    public long Id { get; set; }

    // --- FK ---
    public int UserId { get; set; }
    public int MembershipLevelId { get; set; }

    // --- Identity ---
    public string CardCode { get; set; } = null!;

    // --- Loyalty ---
    public int CurrentCoins { get; set; } = 0;
    public decimal TotalSpent { get; set; } = 0;

    // --- Level lifecycle ---
    public DateOnly? LevelStartDate { get; set; }
    public DateOnly? LevelEndDate { get; set; }
    public DateOnly? LastLevelSpentReset { get; set; }

    public MembershipStatusEnum Status { get; set; } = MembershipStatusEnum.Active;

    // --- Audit ---
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // --- Navigation ---
    public virtual User User { get; set; } = null!;
    public virtual MembershipLevel Level { get; set; } = null!;
    public virtual ICollection<PointHistory> PointHistories { get; set; } = new List<PointHistory>();
}
