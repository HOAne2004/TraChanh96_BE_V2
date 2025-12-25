using drinking_be.Enums;
using drinking_be.Interfaces;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class News : ISoftDelete
{
    public int Id { get; set; }

    public Guid PublicId { get; set; }

    public string Slug { get; set; } = null!;

    public NewsTypeEnum Type { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? ThumbnailUrl { get; set; }

    public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Draft;

    public bool IsFeatured { get; set; } = false;

    public int ViewCount { get; set; } = 0;

    public string? SeoDescription { get; set; }

    public DateTime? PublishedDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual User User { get; set; } = null!;
}
