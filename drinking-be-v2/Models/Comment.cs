using drinking_be.Enums;
using System;
using System.Collections.Generic;
using drinking_be.Interfaces;
namespace drinking_be.Models;

public partial class Comment:ISoftDelete
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public int UserId { get; set; }

    public int NewsId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ReviewStatusEnum Status { get; set; } = ReviewStatusEnum.Pending;

    public virtual ICollection<Comment> InverseParent { get; set; } = new List<Comment>();

    public virtual News News { get; set; } = null!;

    public virtual Comment? Parent { get; set; }

    public virtual User User { get; set; } = null!;
}
