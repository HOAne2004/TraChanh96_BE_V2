using drinking_be.Enums;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class CartItem
{
    public long Id { get; set; }

    public long CartId { get; set; }
    public int ProductId { get; set; }
    public short? SizeId { get; set; }

    public int Quantity { get; set; }

    // Snapshot pricing
    public decimal BasePrice { get; set; }
    public decimal FinalPrice { get; set; }

    public SugarLevelEnum? SugarLevel { get; set; }
    public IceLevelEnum? IceLevel { get; set; }

    public string? Note { get; set; }

    // Combo / Topping
    public long? ParentItemId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public virtual Cart Cart { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual Size? Size { get; set; }

    public virtual CartItem? ParentItem { get; set; }
    public virtual ICollection<CartItem> InverseParentItem { get; set; } = new List<CartItem>();
}
