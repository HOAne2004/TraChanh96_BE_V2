using drinking_be.Enums;
using drinking_be.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace drinking_be.Models;

public partial class Review : ISoftDelete
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    // Liên kết với đơn hàng để xác thực "Đã mua"
    public long OrderId { get; set; }

    public string? Content { get; set; }

    [Range(1, 5)]
    public byte Rating { get; set; }

    public ReviewStatusEnum Status { get; set; } = ReviewStatusEnum.Pending;

    // Giữ lại cái này để lưu ảnh đơn giản (1 ảnh)
    public string? MediaUrl { get; set; }

    public string? AdminResponse { get; set; }

    public bool IsEdited { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; } // ISoftDelete

    // --- Navigation Properties ---
    public virtual Product Product { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual Order Order { get; set; } = null!;
}