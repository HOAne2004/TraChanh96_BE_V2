using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;

public partial class Store : ISoftDelete
{
    public int Id { get; set; }

    public Guid PublicId { get; set; }

    public string? Slug { get; set; }

    public int BrandId { get; set; }

    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public DateTime? OpenDate { get; set; }

    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public double DeliveryRadius { get; set; } = 20;
    public decimal? ShippingFeeFixed { get; set; }
    public decimal? ShippingFeePerKm { get; set; }

    public StoreStatusEnum Status { get; set; } = StoreStatusEnum.ComingSoon;

    public short? SortOrder { get; set; }

    public bool MapVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Mới bổ sung
    public string? Description { get; set; } // Hỗ trợ HTML/Rich Text
    public string? PhoneNumber { get; set; }
    public string? WifiPassword { get; set; }  // Mật khẩu Wifi

    public long AddressId { get; set; }
    public virtual Address Address { get; set; } = null!;

    public virtual Brand Brand { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<ShopTable> ShopTables { get; set; } = new List<ShopTable>();
    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
    public virtual ICollection<SocialMedia> SocialMedias { get; set; } = new List<SocialMedia>();
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    public virtual ICollection<Staff> Staffs { get; set; } = new List<Staff>();
    public virtual ICollection<ProductStore> ProductStores { get; set; } = new List<ProductStore>();
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

}
