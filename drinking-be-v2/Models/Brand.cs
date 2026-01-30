using drinking_be.Enums;
using drinking_be.Models;

public partial class Brand
{
    public int Id { get; set; }

    public Guid PublicId { get; set; }

    public string Name { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public string? Address { get; set; }

    public string? Hotline { get; set; }

    public string? EmailSupport { get; set; }

    public string? TaxCode { get; set; }

    public string? CompanyName { get; set; }

    public string? Slogan { get; set; }

    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

    public string? CopyrightText { get; set; }

    public DateTime? EstablishedDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
    public virtual ICollection<SocialMedia> SocialMedias { get; set; } = new List<SocialMedia>();
    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
