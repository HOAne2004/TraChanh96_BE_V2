using drinking_be.Enums;
using System;
using System.Collections.Generic;
using drinking_be.Interfaces;

namespace drinking_be.Models;

public partial class Review : ISoftDelete
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public string? Content { get; set; }

    public byte Rating { get; set; }

    public ReviewStatusEnum Status { get; set; } = ReviewStatusEnum.Pending;

    public string? MediaUrl { get; set; }

    public string? AdminResponse { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
