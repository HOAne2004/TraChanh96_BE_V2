using drinking_be.Enums;
using System;
using System.Collections.Generic;
using drinking_be.Interfaces;

namespace drinking_be.Models;

public partial class MembershipLevel: ISoftDelete
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal MinSpendRequired { get; set; }

    public short DurationDays { get; set; }

    public string? Benefits { get; set; }

    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public virtual ICollection<VoucherTemplate> VoucherTemplates { get; set; } = new List<VoucherTemplate>();
}
