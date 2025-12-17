using drinking_be.Enums;
using drinking_be.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace drinking_be.Models;

public partial class Product : ISoftDelete
{
    public int Id { get; set; }

    public string PublicId { get; set; } = null!;

    public int CategoryId { get; set; }

    public string Slug { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ProductType { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    public string? Ingredient { get; set; }

    public ProductStatusEnum Status { get; set; } = ProductStatusEnum.Active;

    public double? TotalRating { get; set; }

    public int? TotalSold { get; set; }

    public byte[]? SearchVector { get; set; }

    public DateTime? LaunchDateTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty(nameof(ProductSize.Product))]
    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}
