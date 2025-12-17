using drinking_be.Enums;
using System;
using System.Collections.Generic;
using drinking_be.Interfaces;

namespace drinking_be.Models;

public partial class Membership: ISoftDelete
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public string CardCode { get; set; } = null!;

    public byte LevelId { get; set; }

    public decimal? TotalSpent { get; set; }

    public DateOnly? LevelStartDate { get; set; }

    public DateOnly LevelEndDate { get; set; }

    public DateOnly? LastLevelSpentReset { get; set; }

    public MembershipStatusEnum Status { get; set; } = MembershipStatusEnum.Active;

    public DateTime? CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual MembershipLevel Level { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
