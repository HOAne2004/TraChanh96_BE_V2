using drinking_be.Enums;
using drinking_be.Interfaces;

public partial class ProductSize : ISoftDelete
{
    // --- COMPOSITE KEY ---
    public int ProductId { get; set; }
    public short SizeId { get; set; }

    // --- NGHIỆP VỤ ---
    public decimal? PriceOverride { get; set; }
    // Nếu null → dùng BasePrice + Size.PriceModifier

    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

    public byte? SortOrder { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // --- NAVIGATION ---
    public virtual Product Product { get; set; } = null!;
    public virtual Size Size { get; set; } = null!;
}
