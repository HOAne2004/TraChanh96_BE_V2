using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;

public partial class Product : ISoftDelete
{
    public int Id { get; set; }

    public Guid PublicId { get; set; }
    public int BrandId { get; set; }

    public int CategoryId { get; set; }

    public string Slug { get; set; } = null!;

    public string Name { get; set; } = null!;

    public ProductTypeEnum ProductType { get; set; }

    public decimal BasePrice { get; set; }

    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    public string? Ingredient { get; set; }

    public ProductStatusEnum Status { get; set; } = ProductStatusEnum.Active;

    public double? TotalRating { get; set; }
    public int? TotalSold { get; set; }

    public byte[]? SearchVector { get; set; }

    public DateTime? LaunchDateTime { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // --- NAVIGATION ---
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();

    // ⭐ HOOK CHO STORE (SẼ DÙNG Ở BƯỚC SAU)
    public virtual Brand Brand { get; set; } = null!;
    public virtual ICollection<ProductStore> ProductStores { get; set; } = new List<ProductStore>();
}
