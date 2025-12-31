using drinking_be.Enums;
using drinking_be.Interfaces;
using drinking_be.Models;

public partial class User : ISoftDelete
{
    public int Id { get; set; }

    public Guid PublicId { get; set; }

    public UserRoleEnum RoleId { get; set; } = UserRoleEnum.Customer;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string PasswordHash { get; set; } = null!;

    public bool EmailVerified { get; set; } = false;

    public UserStatusEnum Status { get; set; } = UserStatusEnum.Active;

    public DateTime? LastLogin { get; set; }

    // --- Auth ---
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordTokenExpiryTime { get; set; }

    // --- Audit ---
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // --- Navigation ---
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public virtual Membership? Membership { get; set; }
    public virtual Staff? Staff { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<UserVoucher> UserVouchers { get; set; } = new List<UserVoucher>();
    public virtual ICollection<News> News { get; set; } = new List<News>();
}
