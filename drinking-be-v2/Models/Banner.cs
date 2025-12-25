using drinking_be.Enums;
using drinking_be.Interfaces;

namespace drinking_be.Models;

public class Banner : ISoftDelete
{
    public int Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Title { get; set; }

    public string? LinkUrl { get; set; }

    public int SortOrder { get; set; } = 0;

    public string? Position { get; set; }
    // Ví dụ: "HOME_TOP", "HOME_POPUP", "SIDEBAR"

    public PublicStatusEnum Status { get; set; } = PublicStatusEnum.Active;

    // ⭐ LỊCH HIỂN THỊ
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }

    public bool IsClickable { get; set; } = true;

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
