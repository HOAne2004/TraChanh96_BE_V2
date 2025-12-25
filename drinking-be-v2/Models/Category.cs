using drinking_be.Enums;
using System;
using System.Collections.Generic;
using drinking_be.Interfaces;

namespace drinking_be.Models;

public partial class Category : ISoftDelete
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public string Slug { get; set; } = null!;

    public string Name { get; set; } = null!;

    public byte? SortOrder { get; set; }

    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual Category? Parent { get; set; }
    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

