using drinking_be.Enums;
using System;
using System.Collections.Generic;

namespace drinking_be.Models;

public partial class CartItem
{
    public long Id { get; set; }
    public long CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal BasePrice { get; set; }
    public decimal FinalPrice { get; set; }
    public string? Note { get; set; }
    public long? ParentItemId { get; set; }
    public short? SizeId { get; set; }
    public SugarLevelEnum? SugarLevel { get; set; } = SugarLevelEnum.S100;
    public IceLevelEnum? IceLevel { get; set; } = IceLevelEnum.I100;
    public virtual Cart Cart { get; set; } = null!;
    public virtual ICollection<CartItem> InverseParentItem { get; set; } = new List<CartItem>();
    public virtual CartItem? ParentItem { get; set; }
    public virtual Product Product { get; set; } = null!;
    public virtual Size? Size { get; set; }

}
