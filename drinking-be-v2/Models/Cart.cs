using drinking_be.Enums;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class Cart
{
    public long Id { get; set; }

    public int UserId { get; set; }
    public int StoreId { get; set; }

    public CartStatusEnum Status { get; set; } = CartStatusEnum.Active;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;
    public virtual Store Store { get; set; } = null!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
