using drinking_be.Enums;
using drinking_be.Interfaces;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class Policy : ISoftDelete
{
    public int Id { get; set; }

    public string Slug { get; set; } = null!;

    public int BrandId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public PolicyReviewStatusEnum Status { get; set; } = PolicyReviewStatusEnum.Pending;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual Brand Brand { get; set; } = null!;
    public int? StoreId { get; set; }
    public virtual Store? Store { get; set; }
}
