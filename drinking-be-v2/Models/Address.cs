using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;

public partial class Address : ISoftDelete
{
    public long Id { get; set; }

    public int? UserId { get; set; }
    public int? StoreId { get; set; }

    public string? RecipientName { get; set; }
    public string? RecipientPhone { get; set; }

    public string FullAddress { get; set; } = null!;
    public string AddressDetail { get; set; } = null!;

    public string Province { get; set; } = null!;
    public string District { get; set; } = null!;
    public string Commune { get; set; } = null!;

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public bool? IsDefault { get; set; }

    public PublicStatusEnum? Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual Store? Store { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
