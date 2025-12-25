using drinking_be.Enums;
using drinking_be.Interfaces;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class Comment : ISoftDelete
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public int UserId { get; set; }

    public int NewsId { get; set; }

    public string Content { get; set; } = null!;

    public ReviewStatusEnum  Status { get; set; } = ReviewStatusEnum.Pending;

    public bool IsEdited { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;
    public virtual News News { get; set; } = null!;

    public virtual Comment? Parent { get; set; }
    public virtual ICollection<Comment> InverseParent { get; set; } = new List<Comment>();
}
